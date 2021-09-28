using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.User;

namespace API.Controllers
{
    // TODO - remove coupling between API and Envity by moving User entity somewhere in Application layer
    [Route("users")]
    public class UserController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public UserController(IUserService registrationService, IMapper mapper)
        {
            _userService = registrationService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register(UserRegister userToRegister)
        {

            var origin = Request.Headers["origin"];

            //await _userService.RegisterAsync(userToRegister, origin);

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
            await _userService.ConfirmEmailAsync(emailverification);

            return Ok("Email adresa potvrđena. Možete se ulogovati.");
        }

        [HttpGet]
        public async Task<ActionResult<UserCurrentlyLoggedIn>> GetCurrentlyLoggedInUser()
        {
            var userCurrentlyLoggedInUser = await _userService.GetCurrentlyLoggedInUserAsync();

            return userCurrentlyLoggedInUser;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserBaseResponse>> Login(UserLogin userLogin)
        {
            var userReponse = await _userService.LoginAsync(userLogin);

            SetTokenCookie(userReponse.RefreshToken);

            return userReponse;
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

            var userResponse = await _userService.RefreshTokenAsync(refreshToken);

            SetTokenCookie(userResponse.RefreshToken);

            return userResponse;
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
            await _userService.ConfirmUserPasswordRecoveryAsync(passwordRecoveryVerify);
            return Ok("Uspešna izmena šifre. Molimo Vas da se ulogujete sa novim kredencijalima.");
        }

        [HttpPost("changePassword")]
        public async Task<ActionResult> ChangePassword(UserPasswordChange user)
        {
            await _userService.ChangeUserPasswordAsync(user);

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
