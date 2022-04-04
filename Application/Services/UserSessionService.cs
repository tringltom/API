using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces.Security;
using Application.ManagerInterfaces;
using Application.Models.User;
using Application.ServiceInterfaces;
using AutoMapper;
using DAL;
using Domain;


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

        public async Task<UserBaseResponse> GetCurrentlyLoggedInUserAsync(bool stayLoggedIn, string refreshToken)
        {
            var username = _userAccessor.GetUsernameFromAccesssToken();

            User user;

            if (username != null)
            {
                user = await _userManager.FindUserByNameAsync(username);
            }
            else
            {
                if (stayLoggedIn)
                {
                    var oldRefreshToken = await _uow.RefreshTokens.GetOldRefreshToken(refreshToken);

                    if (oldRefreshToken != null && !oldRefreshToken.IsActive)
                        throw new RestException(HttpStatusCode.Unauthorized, new { Greska = "Niste autorizovani." });

                    user = oldRefreshToken.User;
                }
                else
                {
                    return null;
                }
            }

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Username = "Greška, korisnik sa datim korisničkim imenom nije pronađen." });

            var userResponse = _mapper.Map<UserBaseResponse>(user);

            userResponse.Token = _tokenManager.CreateJWTToken(user.Id, user.UserName);
            userResponse.ActivityCounts = await _activityCounterManager.GetActivityCountsAsync(user);

            return userResponse;
        }

        public async Task<UserBaseResponse> LoginAsync(UserLogin userLogin)
        {
            var user = await _userManager.FindUserByEmailAsync(userLogin.Email);

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Nevalidan email ili nevalidna šifra." });

            if (!user.EmailConfirmed)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Molimo potvrdite Vašu email adresu pre logovanja." });

            var signInResult = await _userManager.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password);

            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                    throw new RestException(HttpStatusCode.Unauthorized, new { Greska = $"Vaš nalog je zaključan. Pokušajte ponovo za {Convert.ToInt32((user.LockoutEnd?.UtcDateTime - DateTime.UtcNow)?.TotalMinutes)} minuta." });
                else
                    throw new RestException(HttpStatusCode.Unauthorized, new { Greska = "Nevalidan email ili nevalidna šifra." });
            }

            var refreshToken = new RefreshToken() { Token = _tokenManager.CreateRefreshToken() };

            user.RefreshTokens.Add(refreshToken);

            if (!await _userManager.UpdateUserAsync(user))
                throw new RestException(HttpStatusCode.InternalServerError, new { Greska = $"Neuspešna izmena za korisnika {user.UserName}." });

            var userResponse = _mapper.Map<UserBaseResponse>(user);

            userResponse.Token = _tokenManager.CreateJWTToken(user.Id, user.UserName);
            userResponse.ActivityCounts = await _activityCounterManager.GetActivityCountsAsync(user);
            userResponse.RefreshToken = refreshToken.Token;

            return userResponse;
        }

        public async Task<UserRefreshResponse> RefreshTokenAsync(string refreshToken)
        {
            var currentUserName = _userAccessor.GetUsernameFromAccesssToken();

            var user = await _userManager.FindUserByNameAsync(currentUserName);

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = $"Nije pronađen korisnik sa korisničkim imenom {currentUserName}." });

            var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

            if (oldToken != null && !oldToken.IsActive)
                throw new RestException(HttpStatusCode.Unauthorized, new { Greska = "Niste autorizovani." });

            if (oldToken != null)
                oldToken.Revoked = DateTimeOffset.UtcNow;

            var newRefreshToken = new RefreshToken() { Token = _tokenManager.CreateRefreshToken() };

            user.RefreshTokens.Add(newRefreshToken);

            if (!await _userManager.UpdateUserAsync(user))
                throw new RestException(HttpStatusCode.InternalServerError, new { Greska = $"Neuspešna izmena za korisnika {user.UserName}." });


            var userToken = _tokenManager.CreateJWTToken(user.Id, user.UserName);

            return new UserRefreshResponse(userToken, newRefreshToken.Token);
        }

        public async Task LogoutUserAsync(string refreshToken)
        {
            var currentUserName = _userAccessor.GetUsernameFromAccesssToken();

            var user = await _userManager.FindUserByNameAsync(currentUserName);

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = $"Nije pronađen korisnik sa korisničkim imenom {currentUserName}." });

            var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

            if (oldToken != null && !oldToken.IsActive)
                throw new RestException(HttpStatusCode.Unauthorized, new { Error = "Niste autorizovani." });

            if (oldToken != null)
                oldToken.Revoked = DateTimeOffset.UtcNow;

            if (!await _userManager.UpdateUserAsync(user))
                throw new RestException(HttpStatusCode.InternalServerError, new { Error = $"Neuspešna izmena za korisnika {user.UserName}." });
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
