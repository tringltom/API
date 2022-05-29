using System.Threading.Tasks;
using API.Validations;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using DAL.Query;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("activities")]
    public class ActivityController : BaseController
    {
        private readonly IActivityService _activityService;

        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpGet("{id}", Name = nameof(GetActivity))]
        [IdValidation]
        public async Task<IActionResult> GetActivity(int id)
        {
            return Ok(await _activityService.GetActivityAsync(id));
        }

        [HttpGet("others")]
        public async Task<IActionResult> GetActivitiesFromOtherUsers([FromQuery] ActivityQuery activityQuery)
        {
            return Ok(await _activityService.GetActivitiesFromOtherUsersAsync(activityQuery));
        }

        // TODO - Add checking if user is Admin
        [HttpGet("pending-happenings")]
        public async Task<IActionResult> GetHappeningsForApproval([FromQuery] QueryObject queryObject)
        {
            return Ok(await _activityService.GetHappeningsForApprovalAsync(queryObject));
        }

        [HttpPatch("{id}/answer")]
        [IdValidation]
        public async Task<IActionResult> AnswerToPuzzle(int id, PuzzleAnswer puzzleAnswer)
        {
            var result = await _activityService.AnswerToPuzzleAsync(id, puzzleAnswer);

            return result.Match(
               xpReward => Ok(xpReward),
               err => err.Response()
               );
        }

        //TO DO - add qr code specific token 
        [HttpPatch("{id}/attendence-confirmation")]
        [IdValidation]
        public async Task<IActionResult> ConfirmAttendenceToHappening(int id)
        {
            var result = await _activityService.ConfirmAttendenceToHappeningAsync(id);

            return result.Match(
               u => Ok(),
               err => err.Response()
               );
        }

        [HttpPatch("{id}/happening-completion-approval")]
        [IdValidation]
        public async Task<IActionResult> ApproveHappeningCompletition(int id, HappeningApprove happeningApprove)
        {
            var result = await _activityService.ApproveHappeningCompletitionAsync(id, happeningApprove.Approve);

            return result.Match(
               u => Ok(),
               err => err.Response()
               );
        }

        [HttpDelete("{id}/attendence")]
        [IdValidation]
        public async Task<IActionResult> CancelAttendenceToHappening(int id)
        {
            var result = await _activityService.AttendToHappeningAsync(id, false);

            return result.Match(
               u => Ok(),
               err => err.Response()
               );
        }

        // TODO - Add checking if user is Admin/Approver
        [HttpPost("pending-activity/{id}")]
        [IdValidation]
        public async Task<IActionResult> ApprovePendingActivity(int id)
        {
            var result = await _activityService.ApprovePendingActivity(id);

            return result.Match(
                activity => CreatedAtRoute(nameof(GetActivity), new { id = activity.Id }, activity),
                err => err.Response()
                );
        }

        [HttpPost("{id}/attendence")]
        [IdValidation]
        public async Task<IActionResult> AttendToHappening(int id)
        {
            var result = await _activityService.AttendToHappeningAsync(id, true);

            return result.Match(
               u => Ok(),
               err => err.Response()
               );
        }

        [HttpPost("{id}/happening-completion")]
        [IdValidation]
        public async Task<IActionResult> CompleteHappening(int id, [FromForm] HappeningUpdate happeningUpdate)
        {
            var result = await _activityService.CompleteHappeningAsync(id, happeningUpdate);

            return result.Match(
               u => Ok(),
               err => err.Response()
               );
        }
    }
}
