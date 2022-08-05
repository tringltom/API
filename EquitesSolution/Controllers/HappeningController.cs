using System.Threading.Tasks;
using API.Validations;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using DAL.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("happenings")]
    public class HappeningController : BaseController
    {
        private readonly IHappeningService _happeningService;

        public HappeningController(IHappeningService happeningService)
        {
            _happeningService = happeningService;
        }

        // TODO - Add checking if user is Admin
        [HttpGet("pending")]
        public async Task<IActionResult> GetHappeningsForApproval([FromQuery] QueryObject queryObject)
        {
            return Ok(await _happeningService.GetHappeningsForApprovalAsync(queryObject));
        }

        //TO DO - add qr code specific token 
        [HttpPatch("{id}/attendence-confirmation")]
        [IdValidation]
        public async Task<IActionResult> ConfirmAttendenceToHappening(int id)
        {
            var result = await _happeningService.ConfirmAttendenceToHappeningAsync(id);

            return result.Match(
               u => Ok(),
               err => err.Response()
               );
        }

        [HttpPatch("{id}/completion-approval")]
        [IdValidation]
        public async Task<IActionResult> ApproveHappeningCompletition(int id, HappeningApprove happeningApprove)
        {
            var result = await _happeningService.ApproveHappeningCompletitionAsync(id, happeningApprove.Approve);

            return result.Match(
               u => Ok(),
               err => err.Response()
               );
        }

        [HttpDelete("{id}/attendence")]
        [IdValidation]
        public async Task<IActionResult> CancelAttendenceToHappening(int id)
        {
            var result = await _happeningService.AttendToHappeningAsync(id, false);

            return result.Match(
               u => Ok(),
               err => err.Response()
               );
        }

        [HttpPost("{id}/attendence")]
        [IdValidation]
        public async Task<IActionResult> AttendToHappening(int id)
        {
            var result = await _happeningService.AttendToHappeningAsync(id, true);

            return result.Match(
               u => Ok(),
               err => err.Response()
               );
        }

        [HttpPost("{id}/completion")]
        [IdValidation]
        [AllowAnonymous]
        public async Task<IActionResult> CompleteHappening(int id, [FromForm] HappeningUpdate happeningUpdate)
        {
            var result = await _happeningService.CompleteHappeningAsync(id, happeningUpdate);

            return result.Match(
               u => Ok(),
               err => err.Response()
               );
        }
    }
}
