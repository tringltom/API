using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces;
using Application.ServiceInterfaces;
using DAL;
using DAL.RepositoryInterfaces;
using Domain;
using Microsoft.AspNetCore.WebUtilities;
using Models.User;



namespace Application.Services
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly InfrastructureInterfaces.IUserManager _userManagerRepository;
        private readonly IEmailManager _emailManager;
        private readonly IUnitOfWork _uow;

        public UserRegistrationService(InfrastructureInterfaces.IUserManager userManagerRepository, IEmailManager emailManager, IUnitOfWork uow)
        {
            _userManagerRepository = userManagerRepository;
            _emailManager = emailManager;
            _uow = uow;
        }

        public async Task RegisterAsync(UserRegister user, string origin)
        {
            if (await _uow.Users.ExistsWithEmailAsync(user.Email))
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Već postoji nalog sa unetom email adresom." });

            if (await _uow.Users.ExistsWithUsernameAsync(user.UserName))
                throw new RestException(HttpStatusCode.BadRequest, new { Username = "Korisničko ime već postoji." });

            var newUser = new User() { UserName = user.UserName, Email = user.Email };

            if (!await _userManagerRepository.CreateUserAsync(newUser, user.Password))
                throw new RestException(HttpStatusCode.BadRequest, new { Greska = "Neuspešno dodavanje korisnika." });

            var token = await GenerateUserTokenForEmailConfirmationAsync(newUser);
            var verifyUrl = GenerateVerifyEmailUrl(origin, token, newUser.Email);

            await _emailManager.SendConfirmationEmailAsync(verifyUrl, newUser.Email);
        }

        public async Task ResendConfirmationEmailAsync(string email, string origin)
        {
            var user = await _userManagerRepository.FindUserByEmailAsync(email);

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Nije pronađen korisnik sa unetom email adresom." });

            var token = await GenerateUserTokenForEmailConfirmationAsync(user);
            var verifyUrl = GenerateVerifyEmailUrl(origin, token, email);

            await _emailManager.SendConfirmationEmailAsync(verifyUrl, user.Email);
        }

        public async Task ConfirmEmailAsync(UserEmailVerification userEmailVerify)
        {
            var user = await _userManagerRepository.FindUserByEmailAsync(userEmailVerify.Email);

            if (user == null)
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Nije pronađen korisnik sa unetom email adresom." });

            var decodedToken = _emailManager.DecodeVerificationToken(userEmailVerify.Token);

            if (!await _userManagerRepository.ConfirmUserEmailAsync(user, decodedToken))
                throw new RestException(HttpStatusCode.InternalServerError, new { Greska = "Neuspešno slanje verifikacionog emaila." });
        }

        private async Task<string> GenerateUserTokenForEmailConfirmationAsync(User user)
        {
            var token = await _userManagerRepository.GenerateUserEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            return token;
        }

        private string GenerateVerifyEmailUrl(string origin, string token, string email)
        {
            return $"{origin}/users/verifyEmail?token={token}&email={email}";
        }
    }
}
