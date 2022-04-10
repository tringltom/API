using System.Threading.Tasks;
using API.Validations;
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

        [HttpGet("{id}", Name = nameof(Activity))]
        [IdValidation]
        public async Task<IActionResult> Activity(int id)
        {
            var activity = await _activityService.GetActivityAsync(id);
            return Ok(activity);
        }

        [HttpGet("others")]
        public async Task<IActionResult> ActivitiesFromOtherUsers(int? limit, int? offset)
        {
            var activities = await _activityService.GetActivitiesFromOtherUsersAsync(limit, offset);
            return Ok(activities);
        }

        // TODO - Add checking if user is Admin/Approver
        [HttpPost("pending-activity/{id}")]
        [IdValidation]
        public async Task<IActionResult> ApprovePendingActivity(int id)
        {
            var result = await _activityService.ApprovePendingActivity(id);

            return result.Match(
                activity => CreatedAtRoute(nameof(Activity), new { activity = activity.Id }, activity),
                err => err.Response()
                );
        }
    }
}
