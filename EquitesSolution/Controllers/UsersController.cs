using System;
using System.Threading;
using System.Threading.Tasks;
using API.DTOs.User;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{

    [Route("users")]
    public class UsersController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public UsersController(IUserService registrationService, IMapper mapper)
        {
            _userService = registrationService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register(UserForRegistrationRequestDto userToRegister)
        {
            var user = _mapper.Map<User>(userToRegister);
            var origin = Request.Headers["origin"];

            // await _userService.RegisterAsync(user, userToRegister.Password, origin);

            return Ok("Registracija uspešna - Molimo proverite Vaše poštansko sanduče.");
        }

        [AllowAnonymous]
        [HttpGet("resendEmailVerification")]
        public async Task<ActionResult> ResendEmailVerification([FromQuery] UserForResendEmailVerificationRequestDto user)
        {
            var origin = Request.Headers["origin"];

            await _userService.ResendConfirmationEmailAsync(user.Email, origin);

            return Ok("Email za potvrdu poslat - Molimo proverite Vaše poštansko sanduče.");
        }

        [HttpPost("verifyEmail")]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyEmail(UserForEmailVerificationRequestDto emailverification)
        {
            await _userService.ConfirmEmailAsync(emailverification.Email, emailverification.Token);

            return Ok("Email adresa potvrđena. Možete se ulogovati.");
        }

        [HttpGet]
        public async Task<ActionResult<UserForCurrentlyLoggedInUserResponseDto>> GetCurrentlyLoggedInUser()
        {
            var result = await _userService.GetCurrentlyLoggedInUserAsync();

            var user = _mapper.Map<UserForCurrentlyLoggedInUserResponseDto>(result);

            return user;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserBaseResponseDto>> Login(UserForLoginRequestDto userLogin)
        {
            var result = await _userService.LoginAsync(userLogin.Email, userLogin.Password);

            var user = _mapper.Map<UserBaseResponseDto>(result);

            SetTokenCookie(user.RefreshToken);

            return user;
        }

        [AllowAnonymous]
        [HttpPost("facebook")]
        public async Task<ActionResult<UserBaseResponseDto>> FacebookLogin(string accessToken, CancellationToken cancellationToken)
        {
            //var result = await _userService.FacebookLoginAsync(accessToken, cancellationToken);

            //var user = _mapper.Map<UserBaseResponseDto>(result);

            //SetTokenCookie(user.RefreshToken);
            //return user;

            return StatusCode(500, "Not yet implemented. Application is not enlisted with FB.");
        }

        [HttpPost("refreshToken")]
        public async Task<ActionResult<UserBaseResponseDto>> RefreshToken()
        {

            var refreshToken = Request.Cookies["refreshToken"];

            var result = await _userService.RefreshTokenAsync(refreshToken);

            var user = _mapper.Map<UserBaseResponseDto>(result);

            SetTokenCookie(user.RefreshToken);

            return user;
        }

        [AllowAnonymous]
        [HttpPost("recoverPassword")]
        public async Task<ActionResult> RecoverPassword(UserForRecoverPasswordRequestDto user)
        {
            var origin = Request.Headers["origin"];

            await _userService.RecoverUserPasswordViaEmailAsync(user.Email, origin);

            return Ok("Molimo proverite Vaše poštansko sanduče kako biste uneli novu šifru.");
        }

        [HttpPost("verifyPasswordRecovery")]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyPasswordRecovery(UserForPasswordRecoveryEmailVerificationRequestDto passwordRecoveryVerify)
        {
            await _userService.ConfirmUserPasswordRecoveryAsync(passwordRecoveryVerify.Email, passwordRecoveryVerify.Token, passwordRecoveryVerify.NewPassword);
            return Ok("Uspešna izmena šifre. Molimo Vas da se ulogujete sa novim kredencijalima.");
        }

        [HttpPost("changePassword")]
        public async Task<ActionResult> ChangePassword(UserForPasswordChangeRequestDto user)
        {
            await _userService.ChangeUserPasswordAsync(user.Email, user.OldPassword, user.NewPassword);

            return Ok("Uspešna izmena šifre.");
        }

        private void SetTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
