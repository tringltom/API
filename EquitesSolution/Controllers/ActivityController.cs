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
        private readonly IFavoritesService _favoriteService;

        public ActivityController(IActivityService activityService, IFavoritesService favoritesService)
        {
            _activityService = activityService;
            _favoriteService = favoritesService;
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
    }
}
