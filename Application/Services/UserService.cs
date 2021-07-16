using Application.Errors;
using Application.Models;
using Application.Repositories;
using Application.Security;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {

        private readonly IUserRepository  _userRepository;
        private readonly IEmailService _emailService;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFacebookAccessor _facebookAccessor;

        public UserService(IUserRepository userRepository, IEmailService emailService,
                            IJwtGenerator jwtGenerator, IHttpContextAccessor httpContextAccessor, IFacebookAccessor facebookAccessor)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _jwtGenerator = jwtGenerator;
            _httpContextAccessor = httpContextAccessor;
            _facebookAccessor = facebookAccessor;
        }

        public async Task RegisterAsync(User user, string password, string origin)
        {
            if (await _userRepository.ExistsWithEmailAsync(user.Email))
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Već postoji nalog sa datom email adresom." });

            if (await _userRepository.ExistsWithUsernameAsync(user.UserName))
                throw new RestException(HttpStatusCode.BadRequest, new { Username = "Korisničko ime već postoji." });

            if (!await _userRepository.CreateUserAsync(user, password))
                throw new RestException(HttpStatusCode.BadRequest, new { Greska = "Neuspešno dodavanje korisnika." });

            var token = await GenerateUserTokenForEmailConfirmationAsync(user);
            var verifyUrl = GenerateVerifyEmailUrl(origin, token, user.Email);

            await _emailService.SendConfirmationEmailAsync(verifyUrl, user.Email);
        }

        public async Task ResendConfirmationEmailAsync(string email, string origin)
        {
            var user = await _userRepository.FindUserByEmailAsync(email);

            if(user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Nije pronađen korisnik sa datom email adresom." });

            var token = await GenerateUserTokenForEmailConfirmationAsync(user);
            var verifyUrl = GenerateVerifyEmailUrl(origin, token, email);

            await _emailService.SendConfirmationEmailAsync(verifyUrl, user.Email);
        }

        public async Task ConfirmEmailAsync(string email, string token)
        {
            var user = await _userRepository.FindUserByEmailAsync(email);

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Nije pronađen korisnik sa datom email adresom." });

            var decodedToken = DecodeToken(token);

            if (!await _userRepository.ConfirmUserEmailAsync(user, decodedToken))
                throw new RestException(HttpStatusCode.InternalServerError, new { Error = "Failed to send verification email" });
        }


        public async Task RecoverUserPasswordViaEmailAsync(string email, string origin)
        {
            var user = await _userRepository.FindUserByEmailAsync(email);

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Nije pronađen korisnik sa datom email adresom." });

            var token = await GenerateUserTokenForPasswordResetAsync(user);
            var verifyUrl = GenerateVerifPasswordRecoveryUrl(origin, token, email);

            await _emailService.SendPasswordRecoveryEmailAsync(verifyUrl, user.Email);
        }

        public async Task<User> ConfirmUserPasswordRecoveryAsync(string email, string token, string newPassword)
        {
            var user = await _userRepository.FindUserByEmailAsync(email);

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Nije pronađen korisnik sa datom email adresom." });

            var decodedToken = DecodeToken(token);

            var passwordRecoveryResult = await _userRepository.RecoverUserPasswordAsync(user, decodedToken, newPassword);

            if (!passwordRecoveryResult.Succeeded)
                throw new RestException(HttpStatusCode.InternalServerError, new { Error = $"Failed to update password for user {user.UserName}." });

            return user;
        }

        public async Task<UserBaseServiceResponse> LoginAsync(string email, string password)
        {
            var user = await _userRepository.FindUserByEmailAsync(email);
            
            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Nije pronađen korisnik sa datom email adresom." });

            if (!user.EmailConfirmed)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Molimo potvrdite Vašu email adresu pre logovanja." });

            var signInResult = await _userRepository.SignInUserViaPasswordWithLockoutAsync(user, password);

            if (!signInResult.Succeeded)
            {
                if(signInResult.IsLockedOut)
                {
                    throw new RestException(HttpStatusCode.Unauthorized, new { Error = $"Vaš nalog je zaključan. Pokusajte ponovo u {user.LockoutEnd}." });
                }
                else
                {
                    throw new RestException(HttpStatusCode.Unauthorized, new { Error = "Pogrešna šifra." });
                }
            }

            var refreshToken = _jwtGenerator.GetRefreshToken();

            user.RefreshTokens.Add(refreshToken);

            if (!await _userRepository.UpdateUserAsync(user))
                throw new RestException(HttpStatusCode.InternalServerError, new { Error = $"Failed to update user {user.UserName}." });

            var userToken = _jwtGenerator.CreateToken(user);

            return new UserBaseServiceResponse(userToken, user.UserName, refreshToken.Token);
        }

        public async Task<UserBaseServiceResponse> RefreshTokenAsync(string refreshToken)
        {
            var currentUserName = GetCurrentUsername();

            var user = await _userRepository.FindUserByNameAsync(currentUserName);

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = $"Nije pronađen korisnik sa korisničkim imenom {currentUserName}" });

            var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

            if (oldToken != null && !oldToken.IsActive) { throw new RestException(HttpStatusCode.Unauthorized, new { Error = "Niste autorizovani." }); }

            if (oldToken != null)
            {
                oldToken.Revoked = DateTime.UtcNow;
            }

            var newRefreshToken = _jwtGenerator.GetRefreshToken();

            user.RefreshTokens.Add(newRefreshToken);

            if (!await _userRepository.UpdateUserAsync(user))
                throw new RestException(HttpStatusCode.InternalServerError, new { Error = $"Failed to update user{user.UserName}" });


            var userToken = _jwtGenerator.CreateToken(user);

            return new UserBaseServiceResponse(userToken, user.UserName, newRefreshToken.Token);
        }

        public async Task<UserBaseServiceResponse> FacebookLoginAsync(string accessToken, CancellationToken cancellationToken)
        {
            var userInfo = await _facebookAccessor.FacebookLogin(accessToken);

            if (userInfo == null) 
                throw new RestException(HttpStatusCode.BadRequest, new { User = "Problem tokom validiranja tokena" });

            var user = await _userRepository.FindUserByEmailAsync(userInfo.Email);

            var refreshToken = _jwtGenerator.GetRefreshToken();

            var userToken = _jwtGenerator.CreateToken(user);

            if (user != null)
            {
                user.RefreshTokens.Add(refreshToken);
                if (!await _userRepository.UpdateUserAsync(user))
                    throw new RestException(HttpStatusCode.InternalServerError, new { Error = $"Failed to update user{user.UserName}" });
                
                return new UserBaseServiceResponse(userToken, user.UserName, refreshToken.Token);
            }

            user = new User
            {
                Id = userInfo.Id,
                Email = userInfo.Email,
                UserName = "fb_" + userInfo.Id,
                EmailConfirmed = true
            };

            user.RefreshTokens.Add(refreshToken);

            if (await _userRepository.CreateUserWithoutPasswordAsync(user)) 
                throw new RestException(HttpStatusCode.BadRequest, new { User = "Problem creating user" });

            return new UserBaseServiceResponse(userToken, user.UserName, refreshToken.Token);
        }

        public string GetCurrentUsername()
        {
            var username = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            return username;
        }

        private async Task<string> GenerateUserTokenForEmailConfirmationAsync(User user)
        {
            var token = await _userRepository.GenerateUserEmailConfirmationTokenAsyn(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            return token;
        }

        private async Task<string> GenerateUserTokenForPasswordResetAsync(User user)
        {
            var token = await _userRepository.GenerateUserPasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            return token;
        }

        private string DecodeToken(string token)
        {
            var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            return decodedToken;
        }

        private string GenerateVerifyEmailUrl(string origin, string token, string email)
        {
            return  $"{origin}/user/verifyEmail?token={token}&email={email}";
        }

        private string GenerateVerifPasswordRecoveryUrl(string origin, string token, string email)
        {
            return $"{origin}/user/verifyPasswordRecovery?token={token}&email={email}";
        }

    }
}
