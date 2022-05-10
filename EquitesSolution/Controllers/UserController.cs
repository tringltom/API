using System.Threading.Tasks;
using API.Validations;
using Application.Models;
using Application.Models.User;
using Application.ServiceInterfaces;
using DAL.Query;
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
        public async Task<IActionResult> GetRankedUsers([FromQuery] UserQuery userQuery)
        {
            return Ok(await _usersService.GetRankedUsersAsync(userQuery));
        }

        // TODO - Add checking if user is Admin
        [HttpGet("pending-images")]
        public async Task<IActionResult> GetImagesForApproval([FromQuery] QueryObject queryObject)
        {
            return Ok(await _usersService.GetImagesForApprovalAsync(queryObject));
        }

        [HttpPatch("me/about")]
        public async Task<IActionResult> UpdateAbout(UserAbout user)
        {
            await _usersService.UpdateAboutAsync(user);

            return Ok();
        }

        [HttpPatch("me/image")]
        public async Task<IActionResult> UpdateImage([FromForm] UserImageUpdate userImage)
        {
            var result = await _usersService.UpdateImageAsync(userImage);

            return result.Match(
                u => Ok(),
                err => err.Response());
        }

        // TODO - Add checking if user is Admin
        [HttpPatch("{id}")]
        [IdValidation]
        public async Task<IActionResult> ResolveImage(int id, PhotoApprove photoApprove)
        {
            var result = await _usersService.ResolveImageAsync(id, photoApprove.Approve);

            return result.Match(
                u => Ok(),
                err => err.Response());
        }
    }
}
