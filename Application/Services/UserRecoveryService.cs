using System.Text;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces;
using Application.ManagerInterfaces;
using Application.Models.User;
using Application.ServiceInterfaces;
using LanguageExt;
using Microsoft.AspNetCore.WebUtilities;

namespace Application.Services
{
    public class UserRecoveryService : IUserRecoveryService
    {

        private readonly IUserManager _userManager;
        private readonly IEmailManager _emailManager;

        public UserRecoveryService(IUserManager userManager, IEmailManager emailManager)
        {
            _userManager = userManager;
            _emailManager = emailManager;
        }

        public async Task<Either<RestError, Unit>> RecoverUserPasswordViaEmailAsync(string email, string origin)
        {
            var user = await _userManager.FindUserByEmailAsync(email);

            if (user == null)
                return new BadRequest("Nije pronađen korisnik sa unetom email adresom");

            var token = await _userManager.GenerateUserPasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var verifyUrl = $"{origin}/users/verifyPasswordRecovery?token={token}&email={email}";

            await _emailManager.SendPasswordRecoveryEmailAsync(verifyUrl, user.Email);

            return Unit.Default;
        }

        public async Task<Either<RestError, Unit>> ConfirmUserPasswordRecoveryAsync(UserPasswordRecoveryVerification userPasswordRecovery)
        {
            var user = await _userManager.FindUserByEmailAsync(userPasswordRecovery.Email);

            if (user == null)
                return new BadRequest("Nije pronađen korisnik sa unetom email adresom");

            var decodedToken = _emailManager.DecodeVerificationToken(userPasswordRecovery.Token);

            var passwordRecoveryResult = await _userManager.RecoverUserPasswordAsync(user, decodedToken, userPasswordRecovery.NewPassword);

            if (!passwordRecoveryResult.Succeeded)
                return new InternalServerError("Neuspešna izmena šifre.");

            return Unit.Default;
        }
    }
}
