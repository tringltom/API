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

        [HttpGet("others")]
        public async Task<ActionResult<ApprovedActivityEnvelope>> ActivitiesFromOtherUsers(int? limit, int? offset)
        {
            return await _activityService.GetActivitiesFromOtherUsersAsync(limit, offset);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> Activity(int id)
        {
            return await _activityService.GetActivityAsync(id);
        }

        // TODO - Add checking if user is Admin/Approver
        [HttpPost("pending-activity/{id}")]
        public async Task<ActionResult<Activity>> ApprovePendingActivity(int id)
        {
            return await _activityService.ApprovePendingActivity(id);
        }
    }
}
