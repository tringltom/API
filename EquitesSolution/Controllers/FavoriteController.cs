using System.Collections.Generic;
using System.Threading.Tasks;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using Models.Activity;

namespace API.Controllers
{
    [Route("favorites")]
    public class FavoriteController : BaseController
    {

        private readonly IFavoritesService _favoriteService;

        public FavoriteController(IFavoritesService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpPost("createFavorite")]
        public async Task<ActionResult> CreateFavoriteActivity([FromBody] FavoriteActivityBase favoriteActivityCreate)
        {
            await _favoriteService.CreateFavoriteAsync(favoriteActivityCreate);

            return Ok("Uspešno ste dodali omiljenu aktivnost");
        }

        [HttpPost("removeFavorite")]
        public async Task<ActionResult> RemoveFavoriteActivity([FromBody] FavoriteActivityBase favoriteActivityRemove)
        {
            await _favoriteService.RemoveFavoriteAsync(favoriteActivityRemove);

            return Ok("Uspešno ste uklonili omiljenu aktivnost");
        }

        [HttpGet("{id}")]
        public async Task<IList<FavoriteActivityReturn>> GetFavoriteActivitiesForUser(int id)
        {
            return await _favoriteService.GetAllFavoritesForUserAsync(id);
        }
    }
}
