using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("reviewActivity")]
        public async Task<ActionResult> ReviewActivity([FromBody] ActivityReview activityReview)
        {
            await _reviewService.ReviewActivityAsync(activityReview);

            return Ok("Uspešno ste ocenili aktivnost");
        }

        [HttpGet("getReviewsForUser")]
        [AllowAnonymous]
        public async Task<IList<UserReviewedActivity>> GetReviewsForUser([FromQuery] int userId)
        {
            return await _reviewService.GetAllReviews(userId);
        }
    }
}
