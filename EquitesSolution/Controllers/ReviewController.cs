using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Managers;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Activity;

namespace API.Controllers
{
    [Route("reviews")]
    public class ReviewController : BaseController
    {
        private readonly IReviewManager _reviewManager;
        private readonly IActivityReviewService _activityReviewService;

        public ReviewController(IReviewManager reviewManager, IActivityReviewService activityReviewService)
        {
            _reviewManager = reviewManager;
            _activityReviewService = activityReviewService;
        }

        [HttpPost("reviewActivity")]
        public async Task<ActionResult> ReviewActivity([FromBody] ActivityReview activityReview)
        {
            await _reviewManager.ReviewActivityAsync(activityReview);

            return Ok("Uspešno ste ocenili aktivnost");
        }

        [HttpGet("getReviewsForUser")]
        [AllowAnonymous]
        public async Task<IList<UserReviewedActivity>> GetReviewsForUser([FromQuery] int userId)
        {
            return await _activityReviewService.GetAllReviewsByUserId(userId);
        }
    }
}
