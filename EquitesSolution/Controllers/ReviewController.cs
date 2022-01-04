using System.Threading.Tasks;
using Application.Managers;
using Microsoft.AspNetCore.Mvc;
using Models.Activity;

namespace API.Controllers
{
    [Route("reviews")]
    public class ReviewController : BaseController
    {
        private readonly IReviewManager _reviewManager;

        public ReviewController(IReviewManager reviewManager)
        {
            _reviewManager = reviewManager;
        }

        [HttpPost("reviewActivity")]
        public async Task<ActionResult> ReviewActivity([FromForm] ActivityReview activityReview)
        {
            await _reviewManager.ReviewActivityAsync(activityReview);

            return Ok("Uspešno ste ocenili aktivnost");
        }
    }
}
