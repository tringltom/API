using System.Threading.Tasks;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("pending-activities")]
    public class PendingActivityController : BaseController
    {
        private readonly IActivityService _activityService;

        public PendingActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        // TODO - Add checking if user is Admin
        [HttpGet()]
        public async Task<ActionResult<PendingActivityEnvelope>> PendingActivities(int? limit, int? offset)
        {
            return await _activityService.GetPendingActivitiesAsync(limit, offset);
        }

        [HttpGet("me")]
        public async Task<ActionResult<PendingActivityForUserEnvelope>> OwnerPendingActivities(int? limit, int? offset)
        {
            return await _activityService.GetPendingActivitiesForLoggedInUserAsync(limit, offset);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PendingActivity>> PendingActivitiy(int id)
        {
            return Ok();
            //return await _activityService.GetPendingActivitiesAsync(limit, offset);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PendingActivity>> UpdatePendingActivitiy(PendingActivity pendingActivity)
        {
            return Ok();
            //return await _activityService.GetPendingActivitiesAsync(limit, offset);
        }

        // TODO - Add checking if user is Admin
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePendingActivitiy(int id)
        {
            return Ok();
            //return await _activityService.GetPendingActivitiesAsync(limit, offset);
        }

        [HttpPost()]
        public async Task<ActionResult<PendingActivity>> CreatePendingActivity([FromForm] ActivityCreate activityCreate)
        {
            await _activityService.CreatePendingActivityAsync(activityCreate);

            return Ok("Uspešno kreiranje, molimo Vas da sačekate odobrenje");
        }

    }
}
