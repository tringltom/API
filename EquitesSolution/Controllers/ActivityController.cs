using System.Threading.Tasks;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Activity;

namespace API.Controllers
{
    [Route("activities")]
    public class ActivityController : BaseController
    {
        private readonly IActivityService _activityService;
        private readonly IFavoritesService _favoriteService;
        private readonly IActivityReviewService _activityReviewService;

        public ActivityController(IActivityService activityService, IFavoritesService favoritesService, IActivityReviewService activityReviewService)
        {
            _activityService = activityService;
            _favoriteService = favoritesService;
            _activityReviewService = activityReviewService;
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateActivity([FromForm] ActivityCreate activityCreate)
        {
            await _activityService.CreateActivityAsync(activityCreate);

            return Ok("Uspešno kreiranje, molimo Vas da sačekate odobrenje");
        }

        [HttpPost("createFavorite")]
        public async Task<ActionResult> CreateFavoriteActivity([FromForm] FavoriteActivityCreate favoriteActivityCreate)
        {
            await _favoriteService.CreateFavoriteAsync(favoriteActivityCreate);

            return Ok("Uspešno ste dodali omiljenu aktivnost");
        }

        [HttpPost("removeFavorite")]
        public async Task<ActionResult> RemoveFavoriteActivity([FromForm] FavoriteActivityRemove favoriteActivityRemove)
        {
            await _favoriteService.RemoveFavoriteAsync(favoriteActivityRemove);

            return Ok("Uspešno ste uklonili omiljenu aktivnost");
        }

        [HttpPost("reviewActivity")]
        [AllowAnonymous]
        public async Task<ActionResult> ReviewActivity([FromForm] ActivityReview activityReview)
        {
            await _activityReviewService.ReviewActivityAsync(activityReview);

            return Ok("Uspešno ste ocenili aktivnost");
        }
    }
}
