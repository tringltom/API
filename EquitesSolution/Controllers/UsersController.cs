using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.User;

namespace API.Controllers
{
    // TODO - remove coupling between API and Envity by moving User entity somewhere in Application layer
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
        public async Task<ActionResult> Register(UserRegister userToRegister)
        {
            var user = _mapper.Map<User>(userToRegister);
            var origin = Request.Headers["origin"];

            // await _userService.RegisterAsync(user, userToRegister.Password, origin);

            return Ok("Registracija uspešna - Molimo proverite Vaše poštansko sanduče.");
        }

        [AllowAnonymous]
        [HttpGet("resendEmailVerification")]
        public async Task<ActionResult> ResendEmailVerification([FromQuery] UserEmail user)
        {
            var origin = Request.Headers["origin"];

            await _userService.ResendConfirmationEmailAsync(user.Email, origin);

            return Ok("Email za potvrdu poslat - Molimo proverite Vaše poštansko sanduče.");
        }

        [HttpPost("verifyEmail")]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyEmail(UserEmailVerification emailverification)
        {
            await _userService.ConfirmEmailAsync(emailverification.Email, emailverification.Token);

            return Ok("Email adresa potvrđena. Možete se ulogovati.");
        }

        [HttpGet]
        public async Task<ActionResult<UserCurrentlyLoggedIn>> GetCurrentlyLoggedInUser()
        {
            var result = await _userService.GetCurrentlyLoggedInUserAsync();

            var user = _mapper.Map<UserCurrentlyLoggedIn>(result);

            return user;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserBaseResponse>> Login(UserLogin userLogin)
        {
            var result = await _userService.LoginAsync(userLogin.Email, userLogin.Password);

            var user = _mapper.Map<UserBaseResponse>(result);

            SetTokenCookie(user.RefreshToken);

            return user;
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken != null)
                await _userService.LogoutUserAsync(refreshToken);

            return Ok("Uspešno ste izlogovani.");
        }

        [AllowAnonymous]
        [HttpPost("facebook")]
        public async Task<ActionResult<UserBaseResponse>> FacebookLogin(string accessToken, CancellationToken cancellationToken)
        {
            //var result = await _userService.FacebookLoginAsync(accessToken, cancellationToken);

            //var user = _mapper.Map<UserBaseResponse>(result);

            //SetTokenCookie(user.RefreshToken);
            //return user;

            return StatusCode(500, "Not yet implemented. Application is not enlisted with FB.");
        }

        [HttpPost("refreshToken")]
        public async Task<ActionResult<UserBaseResponse>> RefreshToken()
        {

            var refreshToken = Request.Cookies["refreshToken"];

            var result = await _userService.RefreshTokenAsync(refreshToken);

            var user = _mapper.Map<UserBaseResponse>(result);

            SetTokenCookie(user.RefreshToken);

            return user;
        }

        [AllowAnonymous]
        [HttpPost("recoverPassword")]
        public async Task<ActionResult> RecoverPassword(UserEmail user)
        {
            var origin = Request.Headers["origin"];

            await _userService.RecoverUserPasswordViaEmailAsync(user.Email, origin);

            return Ok("Molimo proverite Vaše poštansko sanduče kako biste uneli novu šifru.");
        }

        [HttpPost("verifyPasswordRecovery")]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyPasswordRecovery(UserPasswordRecoveryVerification passwordRecoveryVerify)
        {
            await _userService.ConfirmUserPasswordRecoveryAsync(passwordRecoveryVerify.Email, passwordRecoveryVerify.Token, passwordRecoveryVerify.NewPassword);
            return Ok("Uspešna izmena šifre. Molimo Vas da se ulogujete sa novim kredencijalima.");
        }

        [HttpPost("changePassword")]
        public async Task<ActionResult> ChangePassword(UserPasswordChange user)
        {
            await _userService.ChangeUserPasswordAsync(user.Email, user.OldPassword, user.NewPassword);

            return Ok("Uspešna izmena šifre.");
        }

        private void SetTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
