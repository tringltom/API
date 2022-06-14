using System.Text;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces;
using Application.ManagerInterfaces;
using Application.Models.User;
using Application.ServiceInterfaces;
using AutoMapper;
using DAL;
using Domain;
using LanguageExt;
using Microsoft.AspNetCore.WebUtilities;

namespace Application.Services
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly IUserManager _userManager;
        private readonly IEmailManager _emailManager;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public UserRegistrationService(IUserManager userManager, IEmailManager emailManager, IUnitOfWork uow, IMapper mapper)
        {
            _userManager = userManager;
            _emailManager = emailManager;
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Either<RestError, Unit>> SendConfirmationEmailAsync(string email, string origin)
        {
            var user = await _userManager.FindUserByEmailAsync(email);

            if (user == null)
                return new NotFound("Nije pronađen korisnik sa unetom email adresom");

            var token = await GenerateUserTokenForEmailConfirmationAsync(user);
            var verifyUrl = GenerateVerifyEmailUrl(origin, token, email);

            await _emailManager.SendConfirmationEmailAsync(verifyUrl, user.Email);
            return Unit.Default;
        }

        public async Task<Either<RestError, UserBaseResponse>> RegisterAsync(UserRegister user, string origin)
        {
            if (await _uow.Users.ExistsWithEmailAsyncAsync(user.Email))
                return new BadRequest("Korisnik sa unetom email adresom već postoji");

            if (await _uow.Users.ExistsWithUsernameAsync(user.UserName))
                return new BadRequest("Korisničko ime već postoji");

            var newUser = new User() { UserName = user.UserName, Email = user.Email };

            if (!await _userManager.CreateUserAsync(newUser, user.Password))
                return new InternalServerError("Neuspešno dodavanje korisnika.");

            var token = await GenerateUserTokenForEmailConfirmationAsync(newUser);
            var verifyUrl = GenerateVerifyEmailUrl(origin, token, newUser.Email);

            await _emailManager.SendConfirmationEmailAsync(verifyUrl, newUser.Email);

            return _mapper.Map<UserBaseResponse>(newUser);
        }

        public async Task<Either<RestError, Unit>> VerifyEmailAsync(UserEmailVerification userEmailVerify)
        {
            var user = await _userManager.FindUserByEmailAsync(userEmailVerify.Email);

            if (user == null)
                return new NotFound("Nije pronađen korisnik sa unetom email adresom.");

            var decodedToken = _emailManager.DecodeVerificationToken(userEmailVerify.Token);

            if (!await _userManager.ConfirmUserEmailAsync(user, decodedToken))
                return new InternalServerError("Neuspešna potvrda email adrese.");

            return Unit.Default;
        }

        private async Task<string> GenerateUserTokenForEmailConfirmationAsync(User user)
        {
            var token = await _userManager.GenerateUserEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            return token;
        }

        private string GenerateVerifyEmailUrl(string origin, string token, string email)
        {
            return $"{origin}/users/verifyEmail?token={token}&email={email}";
        }
    }
}
