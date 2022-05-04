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

        [HttpGet("{id}", Name = nameof(GetActivity))]
        [IdValidation]
        public async Task<IActionResult> GetActivity(int id)
        {
            return Ok(await _activityService.GetActivityAsync(id));
        }

        [HttpGet("others")]
        public async Task<IActionResult> GetActivitiesFromOtherUsers(int? limit, int? offset)
        {
            return Ok(await _activityService.GetActivitiesFromOtherUsersAsync(limit, offset));
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
    }
}
