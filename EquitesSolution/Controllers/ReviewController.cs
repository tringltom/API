using System.Threading.Tasks;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Activity;

namespace API.Controllers
{
    [Route("reviews")]
    public class ReviewController : BaseController
    {
        private readonly IActivityReviewService _activityReviewService;

        public ReviewController(IActivityReviewService activityReviewService)
        {
            _activityReviewService = activityReviewService;
        }

        [HttpPost("reviewActivity")]
        public async Task<ActionResult> ReviewActivity([FromForm] ActivityReview activityReview)
        {
            await _activityReviewService.ReviewActivityAsync(activityReview);

            return Ok("Uspešno ste ocenili aktivnost");
        }
    }
}
