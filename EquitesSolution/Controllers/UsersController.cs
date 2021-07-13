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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        public async Task<ActionResult> Register([FromBody] UserForRegistrationRequestDto userToRegister)
        {
            var user = _mapper.Map<User>(userToRegister);
            var origin = Request.Headers["origin"];

            await _userService.RegisterAsync(user, userToRegister.Password, origin);

            return Ok("Registracija uspesna - Molimo proverite vase postansko sanduce");
        }

        [AllowAnonymous]
        [HttpGet("resendEmailVerification")]
        public async Task<ActionResult> ResendEmailVerification([FromQuery]string email)
        {
            var origin = Request.Headers["origin"];

            await _userService.ResendConfirmationEmailAsync(email, origin);

            return Ok("Email za potvrdu poslat - Molimo proverite vase postansko sanduce");
        }

        [HttpPost("verifyEmail")]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyEmail(UserEmailForVerificationRequestDto emailverification)
        {
            await _userService.ConfirmEmailAsync(emailverification.Email, emailverification.Token);

            return Ok("Email adresa potvrdjena. Mozete se ulogovati");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserBaseResponseDto>> Login(UserForLoginRequestDto userLogin)
        {
            var result = await _userService.LoginAsync(userLogin.Email, userLogin.Password);

            var user = _mapper.Map<UserBaseResponseDto>(result);

            SetTokenCookie(user.RefreshToken);

            return Ok("Uspesno logovanje");
        }

        [AllowAnonymous]
        [HttpPost("facebook")]
        public async Task<ActionResult<UserBaseResponseDto>> FacebookLogin(string accessToken, CancellationToken cancellationToken)
        {

            var result = await _userService.FacebookLoginAsync(accessToken, cancellationToken);

            var user = _mapper.Map<UserBaseResponseDto>(result);

            SetTokenCookie(user.RefreshToken);
            return user;
        }

        [HttpPost("refreshToken")]
        public async Task<ActionResult<UserBaseResponseDto>> RefreshToken(string refreshToken)
        {

            var result = await _userService.RefreshTokenAsync(refreshToken);

            var user = _mapper.Map<UserBaseResponseDto>(result);

            SetTokenCookie(user.RefreshToken);

            return user;
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
