using Application.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IEmailService _emailService;
        private readonly UserManager<User> _userIdentityManager;
        private readonly IUserRepository  _userRepository;

        public RegistrationService(IUserRepository userRepository, IEmailService emailService, UserManager<User> userIdentityManager)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _userIdentityManager = userIdentityManager;
        }

        public async Task Register(User user, string password, string origin)
        {

            if (!await _userRepository.CreateUserAsync(user, password))
            {
                throw new Exception("Neuspesno dodavanje korisnika");
            }

            var token = await GenerateUserTokenForEmailConfirmation(user);
            var verifyUrl = GenerateVerifyUrl(origin, token, user.Email);

            if (!await _emailService.SendConfirmationEmailAsync(verifyUrl, user.Email))
            {
                throw new Exception("Greska pri slanju poste za potvrdu naloga.");
            }
        }

        public async Task ResendConfirmationEmail(string email, string origin)
        {
            var user = await _userRepository.FindUserByEmailAsync(email);

            if(user == null)
            {
                throw new Exception($"Nije pronadjen korisnik sa email adresom {email}");
            }

            var token = await GenerateUserTokenForEmailConfirmation(user);
            var verifyUrl = GenerateVerifyUrl(origin, token, email);

            if (!await _emailService.SendConfirmationEmailAsync(verifyUrl, user.Email))
            {
                throw new Exception("Greska pri slanju poste za potvrdu naloga.");
            }
        }

        private async Task<string> GenerateUserTokenForEmailConfirmation(User user)
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
