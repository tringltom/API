using System;
using System.Threading.Tasks;
using API.DTOs.User;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
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
            // should add fluent validation to save code space
            if (userToRegister == null || !ModelState.IsValid)
            {
                return BadRequest("Unos nije validan");
            }

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
            if (emailverification == null || !ModelState.IsValid)
            {
                return BadRequest("Unos nije validan");
            }

            await _userService.ConfirmEmailAsync(emailverification.Email, emailverification.Token);

            return Ok("Email adresa potvrdjena. Mozete se ulogovati");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(UserForLoginRequestDto userLogin)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Unos nije validan");
            }

            await _userService.LoginAsync(userLogin.Email, userLogin.Password);

            return Ok("Uspesno logovanje");
        }

        //[AllowAnonymous]
        //[HttpPost("facebook")]
        //public async Task<ActionResult<User>> FacebookLogin()
        //{

        //}

        //[HttpPost("refreshToken")]
        //public async Task<ActionResult<User>> RefreshToken()
        //{

        //}

    }
}
