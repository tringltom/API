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

        [HttpGet("me/challenge-answers/activity/{id}")]
        [IdValidation]
        public async Task<IActionResult> GetOwnerChallengeAnswers(int id, [FromQuery] QueryObject queryObject)
        {
            var result = await _activityService.GetOwnerChallengeAnswersAsync(id, queryObject);

            return result.Match(
                challengeAnswers => Ok(challengeAnswers),
                err => err.Response()
                );
        }

        // TODO - Add checking if user is Admin
        [HttpGet("pending-challenges")]
        public async Task<IActionResult> GetChallengesForApproval([FromQuery] QueryObject queryObject)
        {
            return Ok(await _activityService.GetChallengesForApprovalAsync(queryObject));
        }

        [HttpGet("approved-activities/user/{id}", Name = nameof(GetApprovedActivitiesCreatedByUser))]
        public async Task<IActionResult> GetApprovedActivitiesCreatedByUser(int id, [FromQuery] UserQuery userQuery)
        {
            return Ok(await _activityService.GetApprovedActivitiesCreatedByUserAsync(id, userQuery));
        }

        [HttpGet("favorited-activities/user/{id}", Name = nameof(GetFavoritedActivitiesByUser))]
        public async Task<IActionResult> GetFavoritedActivitiesByUser(int id, [FromQuery] UserQuery userQuery)
        {
            return Ok(await _activityService.GetFavoritedActivitiesByUserAsync(id, userQuery));
        }

        [HttpPatch("{id}/puzzle-answer")]
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

        [HttpPatch("challenge-confirmation/{id}")]
        [IdValidation]
        public async Task<IActionResult> ConfirmChallengeAnswer(int id)
        {
            var result = await _activityService.ConfirmChallengeAnswerAsync(id);

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

        // TODO - Add checking if user is Admin
        [HttpPatch("challenge-answer-disapproval/{id}")]
        [IdValidation]
        public async Task<IActionResult> DisapproveChallengeAnswer(int id)
        {
            var result = await _activityService.DisapproveChallengeAnswerAsync(id);

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

        [HttpPost("{id}/challenge-answer")]
        [IdValidation]
        public async Task<IActionResult> AnswerToChallenge(int id, [FromForm] ChallengeAnswer challengeAnswer)
        {
            var result = await _activityService.AnswerToChallengeAsync(id, challengeAnswer);

            return result.Match(
               u => Ok(),
               err => err.Response()
               );
        }

        // TODO - Add checking if user is Admin
        [HttpPost("challenge-answer-approval/{id}")]
        [IdValidation]
        public async Task<IActionResult> ApproveChallengeAnswer(int id)
        {
            var result = await _activityService.ApproveChallengeAnswerAsync(id);

            return result.Match(
               u => Ok(),
               err => err.Response()
               );
        }
    }
}
