using System.Threading.Tasks;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("reviews")]
    public class ReviewController : BaseController
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewManager)
        {
            _reviewService = reviewManager;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetOwnerReviews()
        {
            return Ok(await _reviewService.GetOwnerReviewsAsync());
        }

        [HttpPut()]
        public async Task<IActionResult> ReviewActivity(ActivityReview activityReview)
        {
            var result = await _reviewService.ReviewActivityAsync(activityReview);

            return result.Match(u => Ok(), err => err.Response());
        }
    }
}
