using System.Threading.Tasks;
using API.Validations;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("pending-activities")]
    public class PendingActivityController : BaseController
    {
        private readonly IPendingActivityService _pendinngActivityService;

        public PendingActivityController(IPendingActivityService pendingActivityService)
        {
            _pendinngActivityService = pendingActivityService;
        }

        // TODO - Add checking if user is Admin
        [HttpGet()]
        public async Task<IActionResult> GetPendingActivities(int? limit, int? offset)
        {
            return Ok(await _pendinngActivityService.GetPendingActivitiesAsync(limit, offset));
        }

        [HttpGet("me", Name = nameof(GetOwnerPendingActivities))]
        public async Task<IActionResult> GetOwnerPendingActivities(int? limit, int? offset)
        {
            return Ok(await _pendinngActivityService.GetOwnerPendingActivitiesAsync(limit, offset));
        }

        [HttpGet("me/{id}")]
        [IdValidation]
        public async Task<IActionResult> GetOwnerPendingActivity(int id)
        {
            var result = await _pendinngActivityService.GetOwnerPendingActivityAsync(id);

            return result.Match(
                pendingActivity => Ok(pendingActivity),
                err => err.Response()
                );
        }

        [HttpPut("{id}")]
        [IdValidation]
        public async Task<IActionResult> UpdatePendingActivitiy(int id, ActivityCreate pendingActivity)
        {
            var result = await _pendinngActivityService.UpdatePendingActivityAsync(id, pendingActivity);

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
            var result = await _pendinngActivityService.DisapprovePendingActivityAsync(id);

            return result.Match(
                u => NoContent(),
                err => err.Response()
                );
        }

        [HttpPost()]
        public async Task<IActionResult> CreatePendingActivity([FromForm] ActivityCreate activityCreate)
        {
            var result = await _pendinngActivityService.CreatePendingActivityAsync(activityCreate);

            return result.Match(
               activity => CreatedAtRoute(nameof(GetOwnerPendingActivities), new { activity = activity.Id }, activity),
               err => err.Response()
               );
        }

    }
}
