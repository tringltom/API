using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Models.Activity;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("resolveFavorite")]
        public async Task<ActionResult> ResolveFavoriteActivity([FromBody] FavoriteActivityBase favoriteActivityCreate)
        {
            await _favoriteService.ResolveFavoriteActivityAsync(favoriteActivityCreate);

            return Ok("Uspešno ste dodali omiljenu aktivnost");
        }

        [HttpGet("{id}")]
        public async Task<IList<FavoriteActivityReturn>> GetFavoriteActivitiesForUser(int id)
        {
            return await _favoriteService.GetAllFavoritesForUserAsync(id);
        }
    }
}
