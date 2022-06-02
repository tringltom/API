using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Models.User;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static LanguageExt.Prelude;

namespace API.Controllers
{
    [Route("session")]
    public class SessionController : BaseController
    {
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IUserSessionService _userSessionService;
        private readonly IUserRecoveryService _userRecoveryService;

        public SessionController(IUserRegistrationService registrationService, IUserSessionService userSessionService, IUserRecoveryService userRecoveryService)
        {
            _userRegistrationService = registrationService;
            _userSessionService = userSessionService;
            _userRecoveryService = userRecoveryService;
        }

        [AllowAnonymous]
        [HttpHead("email")]
        public async Task<IActionResult> SendEmailVerification([FromQuery] UserEmail user)
        {
            var result = await _userRegistrationService.SendConfirmationEmailAsync(user.Email, Request.Headers["origin"]);

            return result.Match(
                u => Ok(),
                err => err.Response());
        }

        [AllowAnonymous]
        [HttpHead("password")]
        public async Task<IActionResult> SendRecoverPassword([FromQuery] UserEmail user)
        {
            var result = await _userRecoveryService.RecoverUserPasswordViaEmailAsync(user.Email, Request.Headers["origin"]);

            return result.Match(
                u => Ok(),
                err => err.Response());
        }

        [AllowAnonymous]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentlyLoggedInUser()
        {
            var result = await _userSessionService.GetCurrentlyLoggedInUserAsync(Request.Cookies["stayLoggedIn"], Request.Cookies["refreshToken"]);

            return result.Match(
               user => Ok(user),
               err => err.Response()
               );
        }

        [AllowAnonymous]
        [HttpPut(Name = nameof(Login))]
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            var result = await _userSessionService.LoginAsync(userLogin);

            return result.Match(
               user =>
               {
                   SetTokenCookie(user.RefreshToken, userLogin.StayLoggedIn);
                   return Ok(user);
               },
               err => err.Response()
               );
        }

        [HttpPut("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            var result = await _userSessionService.RefreshTokenAsync(Request.Cookies["refreshToken"]);

            return result.Match(
               userRefresh =>
               {
                   SetTokenCookie(userRefresh.RefreshToken, parseBool(Request.Cookies["stayLoggedIn"]).IfNone(false));
                   return Ok(userRefresh);
               },
               err => err.Response()
               );
        }

        [HttpPatch("email")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail(UserEmailVerification emailverification)
        {
            var result = await _userRegistrationService.VerifyEmailAsync(emailverification);

            return result.Match(u => Ok(), err => err.Response());
        }

        [HttpPatch("password")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyPasswordRecovery(UserPasswordRecoveryVerification passwordRecoveryVerify)
        {
            var result = await _userRecoveryService.ConfirmUserPasswordRecoveryAsync(passwordRecoveryVerify);

            return result.Match(
                u => Ok(),
                err => err.Response());
        }

        [HttpDelete("")]
        public async Task<IActionResult> Logout()
        {
            var result = await _userSessionService.LogoutUserAsync(Request.Cookies["refreshToken"]);

            return result.Match(u => Ok(), err => err.Response());
        }

        [AllowAnonymous]
        [HttpPost("")]
        public async Task<IActionResult> Register(UserRegister userToRegister, string prefix)
        {
            //var result = await _userRegistrationService.RegisterAsync(userToRegister, prefix ?? Request.Headers["origin"]);

            //return result.Match(
            //   user => CreatedAtRoute(nameof(Login), new { user = user.Id }, user),
            //   err => err.Response()
            //   );
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("facebook")]
        public async Task<IActionResult> FacebookLogin(string accessToken, CancellationToken cancellationToken)
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
