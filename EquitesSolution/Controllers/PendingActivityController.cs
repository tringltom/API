using System.Threading.Tasks;
using API.Validations;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using DAL.Query;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("pending-activities")]
    public class PendingActivityController : BaseController
    {
        private readonly IPendingActivityService _pendingActivityService;

        public PendingActivityController(IPendingActivityService pendingActivityService)
        {
            _pendingActivityService = pendingActivityService;
        }

        // TODO - Add checking if user is Admin
        [HttpGet()]
        public async Task<IActionResult> GetPendingActivities([FromQuery] QueryObject queryObject)
        {
            return Ok(await _pendingActivityService.GetPendingActivitiesAsync(queryObject));
        }

        [HttpGet("me", Name = nameof(GetOwnerPendingActivities))]
        public async Task<IActionResult> GetOwnerPendingActivities([FromQuery] ActivityQuery activityQuery)
        {
            return Ok(await _pendingActivityService.GetOwnerPendingActivitiesAsync(activityQuery));
        }

        [HttpGet("me/{id}")]
        [IdValidation]
        public async Task<IActionResult> GetOwnerPendingActivity(int id)
        {
            var result = await _pendingActivityService.GetOwnerPendingActivityAsync(id);

            return result.Match(
                pendingActivity => Ok(pendingActivity),
                err => err.Response()
                );
        }

        [HttpPut("{id}")]
        [IdValidation]
        public async Task<IActionResult> UpdatePendingActivitiy(int id, [FromForm] ActivityCreate pendingActivity)
        {
            var result = await _pendingActivityService.UpdatePendingActivityAsync(id, pendingActivity);

            return result.Match(
                activity => CreatedAtRoute(nameof(GetOwnerPendingActivities), new { activity = activity.Id }, activity),
                err => err.Response()
                );
        }

        // TODO - Add checking if user is Admin
        [HttpDelete("{id}")]
        [IdValidation]
        public async Task<IActionResult> DisapprovePendingActivity(int id)
        {
            var result = await _pendingActivityService.DisapprovePendingActivityAsync(id);

            return result.Match(
                u => NoContent(),
                err => err.Response()
                );
        }

        [HttpPost()]
        public async Task<IActionResult> CreatePendingActivity([FromForm] ActivityCreate activityCreate)
        {
            var result = await _pendingActivityService.CreatePendingActivityAsync(activityCreate);

            return result.Match(
               activity => CreatedAtRoute(nameof(GetOwnerPendingActivities), new { id = activity.Id }, activity),
               err => err.Response()
               );
        }

    }
}
