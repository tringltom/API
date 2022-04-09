using System.Threading.Tasks;
using Application.Models;
using Application.Models.User;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("")]
        public async Task<ActionResult<UserRangingEnvelope>> GetTopXpUsers(int? limit, int? offset)
        {
            return await _usersService.GetRangingUsers(limit, offset);
        }

        // TODO - Add checking if user is Admin
        [HttpGet("pending-images")]
        public async Task<ActionResult<UserImageEnvelope>> GetImagesForApproval(int? limit, int? offset)
        {
            return await _usersService.GetImagesForApprovalAsync(limit, offset);
        }

        [HttpPatch("me")]
        public async Task<ActionResult> UpdateLoggedUserAbout(UserAbout user)
        {
            await _usersService.UpdateLoggedUserAboutAsync(user);

            return Ok("Uspešna izmena o korisniku.");
        }

        [HttpPatch("me")]
        public async Task<ActionResult> UpdateLoggedUserImage([FromForm] UserImageUpdate userImage)
        {
            await _usersService.UpdateLoggedUserImageAsync(userImage);

            return Ok("Uspešna izmena profilne slike, molimo Vas sačekajte odobrenje.");
        }

        // TODO - Add checking if user is Admin
        [HttpPatch("/{id}")]
        public async Task<ActionResult<bool>> ResolveUserImage(int id, PhotoApprove photoApprove)
        {
            return await _usersService.ResolveUserImageAsync(id, photoApprove.Approve);
        }
    }
}
