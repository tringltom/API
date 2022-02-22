using System.Threading.Tasks;
using Application.Models.Activity;
using Application.ServiceInterfaces;
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

        [HttpPost("create")]
        public async Task<ActionResult> CreatePendingActivity([FromForm] ActivityCreate activityCreate)
        {
            await _activityService.CreatePendingActivityAsync(activityCreate);

            return Ok("Uspešno kreiranje, molimo Vas da sačekate odobrenje");
        }

        // TODO - Add checking if user is Admin
        [HttpGet("getPending")]
        public async Task<ActionResult<PendingActivityEnvelope>> GetPendingActivities(int? limit, int? offset)
        {
            return await _activityService.GetPendingActivitiesAsync(limit, offset);
        }

        [HttpGet("getUserPending")]
        public async Task<ActionResult<PendingActivityForUserEnvelope>> GetPendingActivitiesForLoggedInUser(int? limit, int? offset)
        {
            return await _activityService.GetPendingActivitiesForLoggedInUserAsync(limit, offset);
        }

        // TODO - Add checking if user is Admin
        [HttpPost("resolve/{id}")]
        public async Task<ActionResult<bool>> ResolvePendingActivity(int id, PendingActivityApproval approval)
        {
            return await _activityService.ReslovePendingActivityAsync(id, approval);
        }

        [HttpGet("approvedActivitiesExcludeUser/{id}")]
        public async Task<ActionResult<ApprovedActivityEnvelope>> GetApprovedActivitiesExcludeUser(int id, int? limit, int? offset)
        {
            return await _activityService.GetApprovedActivitiesFromOtherUsersAsync(id, limit, offset);
        }

    }
}
