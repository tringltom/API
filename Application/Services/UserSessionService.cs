using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces.Security;
using Application.ManagerInterfaces;
using Application.Models.User;
using Application.ServiceInterfaces;
using AutoMapper;
using DAL;
using Domain;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Application.Services
{
    public class UserSessionService : IUserSessionService
    {
        private readonly IUserManager _userManager;
        private readonly IActivityCounterManager _activityCounterManager;
        private readonly ITokenManager _tokenManager;
        private readonly IFacebookAccessor _facebookAccessor;
        private readonly IUserAccessor _userAccessor;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public UserSessionService(IUserManager userManager,
            IActivityCounterManager activityCounterManager,
            ITokenManager tokenManger,
            IFacebookAccessor facebookAccessor,
            IUserAccessor userAccessor,
            IMapper mapper,
            IUnitOfWork uow)
        {
            _userManager = userManager;
            _activityCounterManager = activityCounterManager;
            _tokenManager = tokenManger;
            _facebookAccessor = facebookAccessor;
            _userAccessor = userAccessor;
            _mapper = mapper;
            _uow = uow;
        }

        public async Task<Either<RestError, UserBaseResponse>> GetCurrentlyLoggedInUserAsync(string stayLoggedIn, string refreshToken)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();
            User user = null;

            if (userId > 0)
                user = await _userManager.FindUserByIdAsync(userId);

            if (userId == 0 && parseBool(stayLoggedIn).IfNone(false))
            {
                var oldRefreshToken = await _uow.RefreshTokens.GetOldRefreshTokenAsync(refreshToken);

                if (oldRefreshToken != null && !oldRefreshToken.IsActive)
                    return new NotAuthorized("Niste autorizovani");

                user = oldRefreshToken.User;
            }

            if (user == null)
                return new NotFound("Korisnik nije pronadjen");

            var userResponse = _mapper.Map<UserBaseResponse>(user);
            userResponse.Token = _tokenManager.CreateJWTToken(user.Id, user.UserName);
            userResponse.ActivityCounts = await _activityCounterManager.GetActivityCountsAsync(user);

            return userResponse;
        }

        public async Task<Either<RestError, UserBaseResponse>> LoginAsync(UserLogin userLogin)
        {
            var user = await _userManager.FindUserByEmailAsync(userLogin.Email);

            if (user == null)
                return new NotFound("Nevalidan email ili nevalidna šifra");

            if (!user.EmailConfirmed)
                return new BadRequest("Molimo potvrdite Vašu email adresu pre logovanja");

            var signInResult = await _userManager.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password);

            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                    return new NotAuthorized($"Vaš nalog je zaključan. Pokušajte ponovo za {Convert.ToInt32((user.LockoutEnd?.UtcDateTime - DateTime.UtcNow)?.TotalMinutes)} minuta");
                else
                    return new NotAuthorized("Nevalidan email ili nevalidna šifra");
            }

            var refreshToken = new RefreshToken() { Token = _tokenManager.CreateRefreshToken() };

            user.RefreshTokens.Add(refreshToken);

            await _userManager.UpdateUserAsync(user);

            var userResponse = _mapper.Map<UserBaseResponse>(user);

            userResponse.Token = _tokenManager.CreateJWTToken(user.Id, user.UserName);
            userResponse.ActivityCounts = await _activityCounterManager.GetActivityCountsAsync(user);
            userResponse.RefreshToken = refreshToken.Token;

            return userResponse;
        }

        public async Task<Either<RestError, UserRefreshResponse>> RefreshTokenAsync(string refreshToken)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();
            var user = await _userManager.FindUserByIdAsync(userId);

            if (user == null)
                return new NotFound("Korisnik nije pronadjen");

            var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

            if (oldToken != null && !oldToken.IsActive)
                return new NotAuthorized("Niste autorizovani");

            if (oldToken != null)
                oldToken.Revoked = DateTimeOffset.UtcNow;

            var newRefreshToken = new RefreshToken() { Token = _tokenManager.CreateRefreshToken() };

            user.RefreshTokens.Add(newRefreshToken);

            await _userManager.UpdateUserAsync(user);

            var userToken = _tokenManager.CreateJWTToken(user.Id, user.UserName);

            return new UserRefreshResponse(userToken, newRefreshToken.Token);
        }

        public async Task<Either<RestError, Unit>> LogoutUserAsync(string refreshToken)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();
            var user = await _userManager.FindUserByIdAsync(userId);

            if (user == null)
                return new NotFound($"Korisnik nije pronadjen");

            var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

            if (oldToken != null && !oldToken.IsActive)
                return new NotAuthorized("Niste autorizovani");

            if (oldToken != null)
                oldToken.Revoked = DateTimeOffset.UtcNow;

            await _userManager.UpdateUserAsync(user);

            return Unit.Default;
        }

        //public async Task<UserBaseResponse> FacebookLoginAsync(string accessToken, CancellationToken cancellationToken)
        //{
        //    var userInfo = await _facebookAccessor.FacebookLogin(accessToken);

        //    if (userInfo == null)
        //        throw new RestException(HttpStatusCode.BadRequest, new { User = "Problem tokom validiranja tokena." });

        //    var user = await _userManagerRepository.FindUserByEmailAsync(userInfo.Email);

        //    var refreshToken = _tokenManager.CreateRefreshToken();

        //    var userToken = _tokenManager.CreateJWTToken(user);

        //    if (user != null)
        //    {
        //        user.RefreshTokens.Add(refreshToken);
        //        if (!await _userManagerRepository.UpdateUserAsync(user))
        //            throw new RestException(HttpStatusCode.InternalServerError, new { Greska = $"Neuspešna izmena za korisnika {user.UserName}." });

        //        return new UserBaseResponse(userToken, user.UserName, refreshToken.Token);
        //    }

        //    user = new User
        //    {
        //        Id = userInfo.Id,
        //        Email = userInfo.Email,
        //        UserName = "fb_" + userInfo.Id,
        //        EmailConfirmed = true,
        //        XpLevelId = 1,
        //        CurrentXp = 0
        //    };

        //    user.RefreshTokens.Add(refreshToken);

        //    if (await _userManagerRepository.CreateUserWithoutPasswordAsync(user))
        //        throw new RestException(HttpStatusCode.BadRequest, new { User = "Neuspešno dodavanje korisnika." });

        //    return new UserBaseResponse(userToken, user.UserName, refreshToken.Token);
        //}
    }
}
