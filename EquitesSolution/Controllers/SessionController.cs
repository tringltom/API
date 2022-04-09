using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Models.User;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("session")]
    public class SessionController : BaseController
    {
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IUserSessionService _userSessionService;
        private readonly IUserRecoveryService _userRecoveryService;

        public SessionController(IUserRegistrationService registrationService, IUserSessionService userSessionService, IUserRecoveryService userRecoveryService, IUsersService usersService)
        {
            _userRegistrationService = registrationService;
            _userSessionService = userSessionService;
            _userRecoveryService = userRecoveryService;
        }

        [AllowAnonymous]
        [HttpHead("email")]
        public async Task<ActionResult> SendEmailVerification([FromQuery] UserEmail user)
        {
            var origin = Request.Headers["origin"];

            await _userRegistrationService.ResendConfirmationEmailAsync(user.Email, origin);

            return Ok("Email za potvrdu poslat - Molimo proverite Vaše poštansko sanduče.");
        }

        [AllowAnonymous]
        [HttpHead("password")]
        public async Task<ActionResult> SendRecoverPassword(UserEmail user)
        {
            var origin = Request.Headers["origin"];

            await _userRecoveryService.RecoverUserPasswordViaEmailAsync(user.Email, origin);

            return Ok("Molimo proverite Vaše poštansko sanduče kako biste uneli novu šifru.");
        }

        [AllowAnonymous]
        [HttpGet()]
        public async Task<ActionResult<UserBaseResponse>> Login(UserLogin userLogin)
        {
            var userReponse = await _userSessionService.LoginAsync(userLogin);

            //move to helper
            SetTokenCookie(userReponse.RefreshToken, userLogin.StayLoggedIn);

            return userReponse;
        }

        [AllowAnonymous]
        [HttpGet("me")]
        public async Task<ActionResult<UserBaseResponse>> GetCurrentlyLoggedInUser()
        {
            bool.TryParse(Request.Cookies["stayLoggedIn"], out var stayLoggedIn);
            var refreshToken = Request.Cookies["refreshToken"];

            return await _userSessionService.GetCurrentlyLoggedInUserAsync(stayLoggedIn, refreshToken);
        }

        [HttpPut("")]
        public async Task<ActionResult<UserRefreshResponse>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            bool.TryParse(Request.Cookies["stayLoggedIn"], out var stayLoggedIn);

            var userResponse = await _userSessionService.RefreshTokenAsync(refreshToken);

            SetTokenCookie(userResponse.RefreshToken, stayLoggedIn);

            return userResponse;
        }

        [HttpPatch("")]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyEmail(UserEmailVerification emailverification)
        {
            await _userRegistrationService.ConfirmEmailAsync(emailverification);

            return Ok("Email adresa potvrđena. Možete se ulogovati.");
        }

        [HttpPatch("")]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyPasswordRecovery(UserPasswordRecoveryVerification passwordRecoveryVerify)
        {
            await _userRecoveryService.ConfirmUserPasswordRecoveryAsync(passwordRecoveryVerify);

            return Ok("Uspešna izmena šifre. Molimo Vas da se ulogujete sa novim kredencijalima.");
        }

        [HttpDelete("")]
        public async Task<ActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken != null)
                await _userSessionService.LogoutUserAsync(refreshToken);

            return Ok("Uspešno ste izlogovani.");
        }

        [AllowAnonymous]
        [HttpPost("")]
        public async Task<ActionResult> Register(UserRegister userToRegister)
        {

            var origin = Request.Headers["origin"];

            //await _userRegistrationService.RegisterAsync(userToRegister, origin);

            return Ok("Registracija uspešna - Molimo proverite Vaše poštansko sanduče.");
        }

        [AllowAnonymous]
        [HttpPost("facebook")]
        public async Task<ActionResult<UserBaseResponse>> FacebookLogin(string accessToken, CancellationToken cancellationToken)
        {
            //var result = await _userSessionService.FacebookLoginAsync(accessToken, cancellationToken);

            //var user = _mapper.Map<UserBaseResponse>(result);

            //SetTokenCookie(user.RefreshToken);
            //return user;

            return StatusCode(500, "Not yet implemented. Application is not enlisted with FB.");
        }

        private void SetTokenCookie(string refreshToken, bool stayLoggedIn)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = stayLoggedIn ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.MinValue
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
            Response.Cookies.Append("stayLoggedIn", stayLoggedIn.ToString(), cookieOptions);
        }
    }
}
