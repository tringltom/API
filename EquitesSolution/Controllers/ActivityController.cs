using System.Threading.Tasks;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Activity;

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
        public async Task<ActionResult> CreateActivity([FromForm] ActivityCreate activityCreate)
        {
            await _activityService.CreateActivityAsync(activityCreate);

            return Ok("Uspešno kreiranje, molimo Vas da sačekate odobrenje");
        }

        // TODO - Add checking if user is Admin
        [HttpGet]
        public async Task<ActionResult<PendingActivityEnvelope>> GetPendingActivities(int? limit, int? offset)
        {
            return await _activityService.GetPendingActivitiesAsync(limit, offset);
        }

        // TODO - Add checking if user is Admin
        [HttpPost("resolve/{id}")]
        public async Task<ActionResult<bool>> ResolvePendingActivity(int pendingActivityID, bool approve)
        {
            return await _activityService.ReslovePendingActivityAsync(pendingActivityID, approve);
        }
    }
}
