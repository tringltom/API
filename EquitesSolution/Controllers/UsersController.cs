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
        private readonly IRegistrationService _registrationService;
        public UsersController(IRegistrationService registrationService, IMapper mapper)
        {
            _registrationService = registrationService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] UserForRegistrationRequestDto userToRegister)
        {
            try
            {
                // should add fluent validation to save code space
                if (userToRegister == null || !ModelState.IsValid)
                {
                    //_logger.LogError("");
                    return BadRequest("Unos nije validan");
                }

                var user = _mapper.Map<User>(userToRegister);
                var origin = Request.Headers["origin"];

                await _registrationService.Register(user, userToRegister.Password, origin);

                return Ok("Registracija uspesna - Molimo proverite vase postansko sanduce");
            }
            catch (Exception e)
            {
                //_logger.LogError($"Something went wrong inside CreateOwner action: {ex.Message}");
                return StatusCode(500, e.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("resendEmailVerification")]
        public async Task<ActionResult> ResendEmailVerification([FromQuery]string email)
        {
            try
            {
                var origin = Request.Headers["origin"];

                await _registrationService.ResendConfirmationEmail(email, origin);

                return Ok("Email za potvrdu poslat - Molimo proverite vase postansko sanduce");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        //[HttpPost("verifyEmail")]
        //[AllowAnonymous]
        //public async Task<ActionResult> VerifyEmail()
        //{

        //}

        //[AllowAnonymous]
        //[HttpPost("login")]
        //public async Task<ActionResult<User>> Login()
        //{

        //}

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
