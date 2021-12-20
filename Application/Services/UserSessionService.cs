using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Repositories;
using Application.Security;
using Application.ServiceInterfaces;
using Domain.Entities;
using Models.User;


namespace Application.Services
{
    public class UserSessionService : IUserSessionService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IFacebookAccessor _facebookAccessor;

        public UserSessionService(IUserRepository userRepository, IJwtGenerator jwtGenerator, IFacebookAccessor facebookAccessor)
        {
            _userRepository = userRepository;
            _jwtGenerator = jwtGenerator;
            _facebookAccessor = facebookAccessor;
        }

        public async Task<UserCurrentlyLoggedIn> GetCurrentlyLoggedInUserAsync(bool stayLoggedIn, string refreshToken)
        {
            var username = _userRepository.GetCurrentUsername();

            User user;

            if (username != null)
            {
                user = await _userRepository.FindUserByNameAsync(username);
            }
            else
            {
                if (stayLoggedIn)
                {
                    var oldRefreshToken = await _userRepository.GetOldRefreshToken(refreshToken);

                    if (oldRefreshToken != null && !oldRefreshToken.IsActive)
                        throw new RestException(HttpStatusCode.Unauthorized, new { Greska = "Niste autorizovani." });

                    user = oldRefreshToken.User;
                }
                else
                {
                    return new UserCurrentlyLoggedIn();
                }
            }

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Username = "Greška, korisnik sa datim korisničkim imenom nije pronađen." });

            var token = _jwtGenerator.CreateToken(user);

            return new UserCurrentlyLoggedIn() { Username = user.UserName, Token = token, CurrentLevel = user.XpLevelId, CurrentXp = user.CurrentXp };
        }

        public async Task<UserBaseResponse> LoginAsync(UserLogin userLogin)
        {
            var user = await _userRepository.FindUserByEmailAsync(userLogin.Email);

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Nevalidan email ili nevalidna šifra." });

            if (!user.EmailConfirmed)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Molimo potvrdite Vašu email adresu pre logovanja." });

            var signInResult = await _userRepository.SignInUserViaPasswordWithLockoutAsync(user, userLogin.Password);

            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut)
                    throw new RestException(HttpStatusCode.Unauthorized, new { Greska = $"Vaš nalog je zaključan. Pokušajte ponovo za {Convert.ToInt32((user.LockoutEnd?.UtcDateTime - DateTime.UtcNow)?.TotalMinutes)} minuta." });
                else
                    throw new RestException(HttpStatusCode.Unauthorized, new { Greska = "Nevalidan email ili nevalidna šifra." });
            }

            var refreshToken = _jwtGenerator.GetRefreshToken();

            user.RefreshTokens.Add(refreshToken);

            if (!await _userRepository.UpdateUserAsync(user))
                throw new RestException(HttpStatusCode.InternalServerError, new { Greska = $"Neuspešna izmena za korisnika {user.UserName}." });

            var userToken = _jwtGenerator.CreateToken(user);

            return new UserBaseResponse(userToken, user.UserName, refreshToken.Token, user.XpLevelId, user.CurrentXp);
        }

        public async Task<UserBaseResponse> RefreshTokenAsync(string refreshToken)
        {
            var currentUserName = _userRepository.GetCurrentUsername();

            var user = await _userRepository.FindUserByNameAsync(currentUserName);

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = $"Nije pronađen korisnik sa korisničkim imenom {currentUserName}." });

            var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

            if (oldToken != null && !oldToken.IsActive)
                throw new RestException(HttpStatusCode.Unauthorized, new { Greska = "Niste autorizovani." });

            if (oldToken != null)
                oldToken.Revoked = DateTimeOffset.UtcNow;

            var newRefreshToken = _jwtGenerator.GetRefreshToken();

            user.RefreshTokens.Add(newRefreshToken);

            if (!await _userRepository.UpdateUserAsync(user))
                throw new RestException(HttpStatusCode.InternalServerError, new { Greska = $"Neuspešna izmena za korisnika {user.UserName}." });


            var userToken = _jwtGenerator.CreateToken(user);

            return new UserBaseResponse(userToken, user.UserName, newRefreshToken.Token);
        }

        public async Task LogoutUserAsync(string refreshToken)
        {
            var currentUserName = _userRepository.GetCurrentUsername();

            var user = await _userRepository.FindUserByNameAsync(currentUserName);

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = $"Nije pronađen korisnik sa korisničkim imenom {currentUserName}." });

            var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

            if (oldToken != null && !oldToken.IsActive)
                throw new RestException(HttpStatusCode.Unauthorized, new { Error = "Niste autorizovani." });

            if (oldToken != null)
                oldToken.Revoked = DateTimeOffset.UtcNow;

            if (!await _userRepository.UpdateUserAsync(user))
                throw new RestException(HttpStatusCode.InternalServerError, new { Error = $"Neuspešna izmena za korisnika {user.UserName}." });
        }

        public async Task<UserBaseResponse> FacebookLoginAsync(string accessToken, CancellationToken cancellationToken)
        {
            var userInfo = await _facebookAccessor.FacebookLogin(accessToken);

            if (userInfo == null)
                throw new RestException(HttpStatusCode.BadRequest, new { User = "Problem tokom validiranja tokena." });

            var user = await _userRepository.FindUserByEmailAsync(userInfo.Email);

            var refreshToken = _jwtGenerator.GetRefreshToken();

            var userToken = _jwtGenerator.CreateToken(user);

            if (user != null)
            {
                user.RefreshTokens.Add(refreshToken);
                if (!await _userRepository.UpdateUserAsync(user))
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

            if (await _userRepository.CreateUserWithoutPasswordAsync(user))
                throw new RestException(HttpStatusCode.BadRequest, new { User = "Neuspešno dodavanje korisnika." });

            return new UserBaseResponse(userToken, user.UserName, refreshToken.Token);
        }
    }
}
