using Application.Errors;
using Application.Repositories;
using Application.Security;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {

        private readonly UserManager<User> _userIdentityManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserRepository  _userRepository;
        private readonly IEmailService _emailService;
        private readonly IJwtGenerator _jwtGenerator;

        public UserService(IUserRepository userRepository, IEmailService emailService, UserManager<User> userIdentityManager,
                            SignInManager<User> signInManager, IJwtGenerator jwtGenerator)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _userIdentityManager = userIdentityManager;
            _signInManager = signInManager;
            _jwtGenerator = jwtGenerator;
        }

        public async Task RegisterAsync(User user, string password, string origin)
        {
            if (await _userRepository.ExistsWithEmailAsync(user.Email))
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Vec postoji nalog sa datom email adresom." });

            if (await _userRepository.ExistsWithUsernameAsync(user.UserName))
                throw new RestException(HttpStatusCode.BadRequest, new { Username = "Korisnicko ime vec postoji." });

            if (!await _userRepository.CreateUserAsync(user, password))
                throw new RestException(HttpStatusCode.BadRequest, new { Greska = "Neuspesno dodavanje korisnika." });

            var token = await GenerateUserTokenForEmailConfirmationAsync(user);
            var verifyUrl = GenerateVerifyUrl(origin, token, user.Email);

            await _emailService.SendConfirmationEmailAsync(verifyUrl, user.Email);
                
        }

        public async Task ResendConfirmationEmailAsync(string email, string origin)
        {
            var user = await _userRepository.FindUserByEmailAsync(email);

            if(user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Nije pronadjen korisnik sa datom email adresom." });

            var token = await GenerateUserTokenForEmailConfirmationAsync(user);
            var verifyUrl = GenerateVerifyUrl(origin, token, email);

            await _emailService.SendConfirmationEmailAsync(verifyUrl, user.Email);
        }

        public async Task ConfirmEmailAsync(string email, string token)
        {
            var user = await _userIdentityManager.FindByEmailAsync(email);

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Nije pronadjen korisnik sa datom email adresom." });

            var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            var result = await _userIdentityManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
                throw new RestException(HttpStatusCode.InternalServerError, new { Error = "Failed to send verification email" });
        }

        public async Task LoginAsync(string email, string password)
        {
            var user = await _userIdentityManager.FindByEmailAsync(email);

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Nije pronadjen korisnik sa datom email adresom." });

            if (!user.EmailConfirmed)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Molimo potvrdite vasu email adresu pre logovanja." });

            var result = await _signInManager
                .CheckPasswordSignInAsync(user, password, false);


            if (!result.Succeeded)
                throw new RestException(HttpStatusCode.Unauthorized, new { Error = "User not authorized." });

            var refreshToken = _jwtGenerator.GetRefreshToken();
            user.RefreshTokens.Add(refreshToken);
            await _userIdentityManager.UpdateAsync(user);
            //return new User(user, _jwtGenerator, refreshToken.Token);
            
        }

        private async Task<string> GenerateUserTokenForEmailConfirmationAsync(User user)
        {
            var token = await _userIdentityManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            return token;
        }

        private string GenerateVerifyUrl(string origin, string token, string email)
        {
            return $"{origin}/user/verifyEmail?token={token}&email={email}";
        }

    }
}
