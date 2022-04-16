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
        private readonly IUsersService _usersService;

        public UserController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetRankedUsers(int? limit, int? offset)
        {
            return Ok(await _usersService.GetRankedUsersAsync(limit, offset));
        }

        // TODO - Add checking if user is Admin
        [HttpGet("pending-images")]
        public async Task<IActionResult> GetImagesForApproval(int? limit, int? offset)
        {
            return Ok(await _usersService.GetImagesForApprovalAsync(limit, offset));
        }

        [HttpPatch("me/about")]
        public async Task<IActionResult> UpdateLoggedUserAbout(UserAbout user)
        {
            await _usersService.UpdateLoggedUserAboutAsync(user);

            return Ok("Uspešna izmena o korisniku.");
        }

        [HttpPatch("me/image")]
        public async Task<IActionResult> UpdateLoggedUserImage([FromForm] UserImageUpdate userImage)
        {
            var result = await _usersService.UpdateLoggedUserImageAsync(userImage);

            return result.Match(
                u => Ok("Uspešna izmena profilne slike, molimo Vas sačekajte odobrenje"),
                err => err.Response());
        }

        // TODO - Add checking if user is Admin
        [HttpPatch("{id}")]
        public async Task<IActionResult> ResolveUserImage(int id, PhotoApprove photoApprove)
        {
            var result = await _usersService.ResolveUserImageAsync(id, photoApprove.Approve);

            return result.Match(
                u => Ok(),
                err => err.Response());
        }
    }
}
