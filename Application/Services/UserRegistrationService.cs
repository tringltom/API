using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Errors;
using Application.Repositories;
using Application.ServiceHelpers;
using Application.ServiceInterfaces;
using Domain.Entities;
using Microsoft.AspNetCore.WebUtilities;
using Models.User;



namespace Application.Services;

public class UserRegistrationService : IUserRegistrationService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IUserServiceHelper _userServiceHelper;

    public UserRegistrationService(IUserRepository userRepository, IEmailService emailService, IUserServiceHelper userServiceHelper)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _userServiceHelper = userServiceHelper;
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

        var decodedToken = _userServiceHelper.DecodeToken(userEmailVerify.Token);

        if (!await _userRepository.ConfirmUserEmailAsync(user, decodedToken))
            throw new RestException(HttpStatusCode.InternalServerError, new { Greska = "Neuspešno slanje verifikacionog emaila." });
    }

    private async Task<string> GenerateUserTokenForEmailConfirmationAsync(User user)
    {
        var token = await _userRepository.GenerateUserEmailConfirmationTokenAsync(user);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        return token;
    }

    private string GenerateVerifyEmailUrl(string origin, string token, string email)
    {
        return $"{origin}/users/verifyEmail?token={token}&email={email}";
    }
}
