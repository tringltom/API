﻿using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Application.Repositories;
using Application.Security;
using Domain.Entities;
using Microsoft.AspNetCore.WebUtilities;
using Models.User;

namespace Application.Services
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IFacebookAccessor _facebookAccessor;

        public UserService(IUserRepository userRepository, IEmailService emailService,
                            IJwtGenerator jwtGenerator, IFacebookAccessor facebookAccessor)
        {
            _userRepository = userRepository;
            _emailService = emailService;
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

            return new UserCurrentlyLoggedIn() { Username = user.UserName, Token = token };
        }

        public async Task RegisterAsync(UserRegister user, string origin)
        {
            if (await _userRepository.ExistsWithEmailAsync(user.Email))
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Već postoji nalog sa unetom email adresom." });

            if (await _userRepository.ExistsWithUsernameAsync(user.UserName))
                throw new RestException(HttpStatusCode.BadRequest, new { Username = "Korisničko ime već postoji." });

            var newUser = new User() { UserName = user.UserName, Email = user.Email };

            if (!await _userRepository.CreateUserAsync(newUser, user.Password))
                throw new RestException(HttpStatusCode.BadRequest, new { Greska = "Neuspešno dodavanje korisnika." });

            var token = await GenerateUserTokenForEmailConfirmationAsync(newUser);
            var verifyUrl = GenerateVerifyEmailUrl(origin, token, newUser.Email);

            await _emailService.SendConfirmationEmailAsync(verifyUrl, newUser.Email);
        }

        public async Task ResendConfirmationEmailAsync(string email, string origin)
        {
            var user = await _userRepository.FindUserByEmailAsync(email);

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Nije pronađen korisnik sa unetom email adresom." });

            var token = await GenerateUserTokenForEmailConfirmationAsync(user);
            var verifyUrl = GenerateVerifyEmailUrl(origin, token, email);

            await _emailService.SendConfirmationEmailAsync(verifyUrl, user.Email);
        }

        public async Task ConfirmEmailAsync(UserEmailVerification userEmailVerify)
        {
            var user = await _userRepository.FindUserByEmailAsync(userEmailVerify.Email);

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Nije pronađen korisnik sa unetom email adresom." });

            var decodedToken = DecodeToken(userEmailVerify.Token);

            if (!await _userRepository.ConfirmUserEmailAsync(user, decodedToken))
                throw new RestException(HttpStatusCode.InternalServerError, new { Greska = "Neuspešno slanje verifikacionog emaila." });
        }


        public async Task RecoverUserPasswordViaEmailAsync(string email, string origin)
        {
            var user = await _userRepository.FindUserByEmailAsync(email);

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Nije pronađen korisnik sa unetom email adresom." });

            var token = await GenerateUserTokenForPasswordResetAsync(user);
            var verifyUrl = GenerateVerifyPasswordRecoveryUrl(origin, token, email);

            await _emailService.SendPasswordRecoveryEmailAsync(verifyUrl, user.Email);
        }

        public async Task ConfirmUserPasswordRecoveryAsync(UserPasswordRecoveryVerification userPasswordRecovery)
        {
            var user = await _userRepository.FindUserByEmailAsync(userPasswordRecovery.Email);

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Nije pronađen korisnik sa unetom email adresom." });

            var decodedToken = DecodeToken(userPasswordRecovery.Token);

            var passwordRecoveryResult = await _userRepository.RecoverUserPasswordAsync(user, decodedToken, userPasswordRecovery.NewPassword);

            if (!passwordRecoveryResult.Succeeded)
                throw new RestException(HttpStatusCode.InternalServerError, new { Greska = "Neuspešna izmena šifre." });
        }

        public async Task ChangeUserPasswordAsync(UserPasswordChange userPassChange)
        {
            var user = await _userRepository.FindUserByEmailAsync(userPassChange.Email);

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Nije pronađen korisnik sa unetom email adresom." });

            var changePassword = await _userRepository.ChangeUserPasswordAsync(user, userPassChange.OldPassword, userPassChange.NewPassword);

            if (!changePassword.Succeeded)
                throw new RestException(HttpStatusCode.InternalServerError, new { Greska = "Neuspešna izmena šifre." });
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

            return new UserBaseResponse(userToken, user.UserName, refreshToken.Token);
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
                EmailConfirmed = true
            };

            user.RefreshTokens.Add(refreshToken);

            if (await _userRepository.CreateUserWithoutPasswordAsync(user))
                throw new RestException(HttpStatusCode.BadRequest, new { User = "Neuspešno dodavanje korisnika." });

            return new UserBaseResponse(userToken, user.UserName, refreshToken.Token);
        }

        private async Task<string> GenerateUserTokenForEmailConfirmationAsync(User user)
        {
            var token = await _userRepository.GenerateUserEmailConfirmationTokenAsync(user);
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
            return $"{origin}/users/verifyEmail?token={token}&email={email}";
        }

        private string GenerateVerifyPasswordRecoveryUrl(string origin, string token, string email)
        {
            return $"{origin}/users/verifyPasswordRecovery?token={token}&email={email}";
        }

    }
}
