using System.Threading.Tasks;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using Domain;
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

        [HttpGet("review")]
        public async Task<ActionResult<ApprovedActivityEnvelope>> ActivitiesForReview(int? limit, int? offset)
        {
            return Ok();
            //return await _activityService.GetApprovedActivitiesFromOtherUsersAsync(id, limit, offset);
        }

        [HttpGet("{id")]
        public async Task<ActionResult<Activity>> Activity(int id)
        {
            return Ok();
            //return await _activityService.GetApprovedActivitiesFromOtherUsersAsync(id, limit, offset);
        }

        // TODO - Add checking if user is Admin/Approver
        [HttpPost("pending-activity/{id}")]
        public async Task<ActionResult<Activity>> ApprovePendingActivity(int id)
        {
            return Ok();
            //return await _activityService.ReslovePendingActivityAsync(id, approval);
        }
    }
}
