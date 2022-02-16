using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces;
using Application.InfrastructureInterfaces.Security;
using Application.ServiceInterfaces;
using DAL;
using DAL.RepositoryInterfaces;
using Domain;
using Models.Activity;
using Models.User;


namespace Application.Services
{
    public class UserSessionService : IUserSessionService
    {
        private readonly IUserManager _userManagerRepository;
        private readonly ITokenManager _tokenManager;
        private readonly IFacebookAccessor _facebookAccessor;
        private readonly IActivityRepository _activityRepository;
        private readonly IUserAccessor _userAccessor;
        private readonly IUnitOfWork _uow;

        public UserSessionService(IUserManager userManagerRepository,
            ITokenManager tokenManger,
            IFacebookAccessor facebookAccessor,
            IActivityRepository activityRepository,
            IUserAccessor userAccessor,
            IUnitOfWork uow)
        {
            _userManagerRepository = userManagerRepository;
            _tokenManager = tokenManger;
            _facebookAccessor = facebookAccessor;
            _activityRepository = activityRepository;
            _userAccessor = userAccessor;
            _uow = uow;
        }

        public int GetUserIdByToken()
        {
            return _userAccessor.GetUserIdFromAccessToken();
        }

        public async Task<UserBaseResponse> GetCurrentlyLoggedInUserAsync(bool stayLoggedIn, string refreshToken)
        {
            var username = _userAccessor.GetUsernameFromAccesssToken();

            User user;

            if (username != null)
            {
                user = await _userManagerRepository.FindUserByNameAsync(username);
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

            var token = _tokenManager.CreateToken(user);

            var activityCounts = await GetAvailableActivitiesCount(user);

            return new UserBaseResponse(token, user.UserName, "", user.XpLevelId, user.CurrentXp, user.LastRollDate, activityCounts, user.Id);
        }

        public async Task<UserBaseResponse> LoginAsync(UserLogin userLogin)
        {
            var user = await _userManagerRepository.FindUserByEmailAsync(userLogin.Email);

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Nevalidan email ili nevalidna šifra." });

            if (!user.EmailConfirmed)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Molimo potvrdite Vašu email adresu pre logovanja." });

            var signInResult = await _userManagerRepository.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password);

            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                    throw new RestException(HttpStatusCode.Unauthorized, new { Greska = $"Vaš nalog je zaključan. Pokušajte ponovo za {Convert.ToInt32((user.LockoutEnd?.UtcDateTime - DateTime.UtcNow)?.TotalMinutes)} minuta." });
                else
                    throw new RestException(HttpStatusCode.Unauthorized, new { Greska = "Nevalidan email ili nevalidna šifra." });
            }

            var refreshToken = _tokenManager.CreateRefreshToken();

            user.RefreshTokens.Add(refreshToken);

            if (!await _userManagerRepository.UpdateUserAsync(user))
                throw new RestException(HttpStatusCode.InternalServerError, new { Greska = $"Neuspešna izmena za korisnika {user.UserName}." });

            var userToken = _tokenManager.CreateToken(user);

            var activityCounts = await GetAvailableActivitiesCount(user);

            return new UserBaseResponse(userToken, user.UserName, refreshToken.Token, user.XpLevelId, user.CurrentXp, user.LastRollDate, activityCounts, user.Id);
        }

        public async Task<UserBaseResponse> RefreshTokenAsync(string refreshToken)
        {
            var currentUserName = _userAccessor.GetUsernameFromAccesssToken();

            var user = await _userManagerRepository.FindUserByNameAsync(currentUserName);

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = $"Nije pronađen korisnik sa korisničkim imenom {currentUserName}." });

            var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

            if (oldToken != null && !oldToken.IsActive)
                throw new RestException(HttpStatusCode.Unauthorized, new { Greska = "Niste autorizovani." });

            if (oldToken != null)
                oldToken.Revoked = DateTimeOffset.UtcNow;

            var newRefreshToken = _tokenManager.CreateRefreshToken();

            user.RefreshTokens.Add(newRefreshToken);

            if (!await _userManagerRepository.UpdateUserAsync(user))
                throw new RestException(HttpStatusCode.InternalServerError, new { Greska = $"Neuspešna izmena za korisnika {user.UserName}." });


            var userToken = _tokenManager.CreateToken(user);

            return new UserBaseResponse(userToken, user.UserName, newRefreshToken.Token);
        }

        public async Task LogoutUserAsync(string refreshToken)
        {
            var currentUserName = _userAccessor.GetUsernameFromAccesssToken();

            var user = await _userManagerRepository.FindUserByNameAsync(currentUserName);

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = $"Nije pronađen korisnik sa korisničkim imenom {currentUserName}." });

            var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

            if (oldToken != null && !oldToken.IsActive)
                throw new RestException(HttpStatusCode.Unauthorized, new { Error = "Niste autorizovani." });

            if (oldToken != null)
                oldToken.Revoked = DateTimeOffset.UtcNow;

            if (!await _userManagerRepository.UpdateUserAsync(user))
                throw new RestException(HttpStatusCode.InternalServerError, new { Error = $"Neuspešna izmena za korisnika {user.UserName}." });
        }

        public async Task<UserBaseResponse> FacebookLoginAsync(string accessToken, CancellationToken cancellationToken)
        {
            var userInfo = await _facebookAccessor.FacebookLogin(accessToken);

            if (userInfo == null)
                throw new RestException(HttpStatusCode.BadRequest, new { User = "Problem tokom validiranja tokena." });

            var user = await _userManagerRepository.FindUserByEmailAsync(userInfo.Email);

            var refreshToken = _tokenManager.CreateRefreshToken();

            var userToken = _tokenManager.CreateToken(user);

            if (user != null)
            {
                user.RefreshTokens.Add(refreshToken);
                if (!await _userManagerRepository.UpdateUserAsync(user))
                    throw new RestException(HttpStatusCode.InternalServerError, new { Greska = $"Neuspešna izmena za korisnika {user.UserName}." });

                return new UserBaseResponse(userToken, user.UserName, refreshToken.Token);
            }

            user = new User
            {
                Id = userInfo.Id,
                Email = userInfo.Email,
                UserName = "fb_" + userInfo.Id,
                EmailConfirmed = true,
                XpLevelId = 1,
                CurrentXp = 0
            };

            user.RefreshTokens.Add(refreshToken);

            if (await _userManagerRepository.CreateUserWithoutPasswordAsync(user))
                throw new RestException(HttpStatusCode.BadRequest, new { User = "Neuspešno dodavanje korisnika." });

            return new UserBaseResponse(userToken, user.UserName, refreshToken.Token);
        }

        // This will be moved to activity service so UserActivityManager can consume it along with GetcurrentUser and Login
        // Also tests will be added after refactoring
        private async Task<List<ActivityCount>> GetAvailableActivitiesCount(User user)
        {
            var activityCounterForDelete = user.ActivityCreationCounters.Where(ac => ac.DateCreated.AddDays(7) < DateTimeOffset.Now).ToList();

            await _activityRepository.DeleteActivityCountersAsync(activityCounterForDelete);

            var usedActivitiesCount = user.ActivityCreationCounters
                .GroupBy(acc => acc.ActivityTypeId)
                .Select(ac => new { Type = ac.Key, UsedCount = ac.Count() }).ToList();

            return Enum.GetValues(typeof(ActivityTypeId)).OfType<ActivityTypeId>()
                .GroupJoin(usedActivitiesCount,
                    atEnum => atEnum,
                    ac => ac.Type,
                    (type, counts) => new ActivityCount
                    {
                        Type = type,
                        Available = 2 - counts.Select(used => used.UsedCount).FirstOrDefault(),
                        Max = 2
                    })
                .ToList();
        }
    }
}
