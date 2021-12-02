using System.Net;
using System.Text;
using Application.Errors;
using Application.Repositories;
using Application.ServiceHelpers;
using Application.ServiceInterfaces;
using Domain.Entities;
using Microsoft.AspNetCore.WebUtilities;
using Models.User;

namespace Application.Services;

public class UserRecoveryService : IUserRecoveryService
{

    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly IUserServiceHelper _userServiceHelper;

    public UserRecoveryService(IUserRepository userRepository, IEmailService emailService, IUserServiceHelper userServiceHelper)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _userServiceHelper = userServiceHelper;
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

        var decodedToken = _userServiceHelper.DecodeToken(userPasswordRecovery.Token);

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

    private async Task<string> GenerateUserTokenForPasswordResetAsync(User user)
    {
        var token = await _userRepository.GenerateUserPasswordResetTokenAsync(user);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        return token;
    }

    private string GenerateVerifyPasswordRecoveryUrl(string origin, string token, string email)
    {
        return $"{origin}/users/verifyPasswordRecovery?token={token}&email={email}";
    }

}
