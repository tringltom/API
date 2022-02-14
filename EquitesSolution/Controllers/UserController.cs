using System;
using System.Threading;
using System.Threading.Tasks;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.User;

namespace API.Controllers
{

    [Route("users")]
    public class UserController : BaseController
    {
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IUserSessionService _userSessionService;
        private readonly IUserRecoveryService _userRecoveryService;
        private readonly IUsersService _usersService;

        public UserController(IUserRegistrationService registrationService, IUserSessionService userSessionService, IUserRecoveryService userRecoveryService, IUsersService usersService)
        {
            _userRegistrationService = registrationService;
            _userSessionService = userSessionService;
            _userRecoveryService = userRecoveryService;
            _usersService = usersService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register(UserRegister userToRegister)
        {

            var origin = Request.Headers["origin"];

            //await _userRegistrationService.RegisterAsync(userToRegister, origin);

            return Ok("Registracija uspešna - Molimo proverite Vaše poštansko sanduče.");
        }

        [AllowAnonymous]
        [HttpGet("resendEmailVerification")]
        public async Task<ActionResult> ResendEmailVerification([FromQuery] UserEmail user)
        {
            var origin = Request.Headers["origin"];

            await _userRegistrationService.ResendConfirmationEmailAsync(user.Email, origin);

            return Ok("Email za potvrdu poslat - Molimo proverite Vaše poštansko sanduče.");
        }

        [HttpPost("verifyEmail")]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyEmail(UserEmailVerification emailverification)
        {
            await _userRegistrationService.ConfirmEmailAsync(emailverification);

            return Ok("Email adresa potvrđena. Možete se ulogovati.");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<UserBaseResponse>> GetCurrentlyLoggedInUser()
        {
            bool.TryParse(Request.Cookies["stayLoggedIn"], out var stayLoggedIn);
            var refreshToken = Request.Cookies["refreshToken"];

            return await _userSessionService.GetCurrentlyLoggedInUserAsync(stayLoggedIn, refreshToken);
        }

        [HttpGet("getTopXpUsers")]
        public async Task<ActionResult<UserArenaEnvelope>> GetTopXpUsers(int? limit, int? offset)
        {
            return await _usersService.GetTopXpUsers(limit, offset);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserBaseResponse>> Login(UserLogin userLogin)
        {
            var userReponse = await _userSessionService.LoginAsync(userLogin);

            SetTokenCookie(userReponse.RefreshToken, userLogin.StayLoggedIn);

            return userReponse;
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken != null)
                await _userSessionService.LogoutUserAsync(refreshToken);

            return Ok("Uspešno ste izlogovani.");
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

        [HttpPost("refreshToken")]
        public async Task<ActionResult<UserBaseResponse>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            bool.TryParse(Request.Cookies["stayLoggedIn"], out var stayLoggedIn);

            var userResponse = await _userSessionService.RefreshTokenAsync(refreshToken);

            SetTokenCookie(userResponse.RefreshToken, stayLoggedIn);

            return userResponse;
        }

        [AllowAnonymous]
        [HttpPost("recoverPassword")]
        public async Task<ActionResult> RecoverPassword(UserEmail user)
        {
            var origin = Request.Headers["origin"];

            await _userRecoveryService.RecoverUserPasswordViaEmailAsync(user.Email, origin);

            return Ok("Molimo proverite Vaše poštansko sanduče kako biste uneli novu šifru.");
        }

        [HttpPost("verifyPasswordRecovery")]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyPasswordRecovery(UserPasswordRecoveryVerification passwordRecoveryVerify)
        {
            await _userRecoveryService.ConfirmUserPasswordRecoveryAsync(passwordRecoveryVerify);

            return Ok("Uspešna izmena šifre. Molimo Vas da se ulogujete sa novim kredencijalima.");
        }

        [HttpPost("changePassword")]
        public async Task<ActionResult> ChangePassword(UserPasswordChange user)
        {
            await _userRecoveryService.ChangeUserPasswordAsync(user);

            return Ok("Uspešna izmena šifre.");
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
