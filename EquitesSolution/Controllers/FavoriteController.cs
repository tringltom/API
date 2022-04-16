﻿using System.Threading.Tasks;
using API.Validations;
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

        [HttpGet("{id}", Name = nameof(GetFavoriteActivity))]
        [IdValidation]
        public async Task<IActionResult> GetFavoriteActivity(int id)
        {
            return Ok(await _favoriteService.GetFavoriteActivityAsync(id));
        }


        [HttpGet("me/ids")]
        public async Task<IActionResult> GetOwnerFavoriteActivityIds()
        {
            return Ok(await _favoriteService.GetAllOwnerFavoriteIds());
        }

        [HttpDelete("{id}")]
        [IdValidation]
        public async Task<IActionResult> RemoveFavouriteActivity(int id)
        {
            var result = await _favoriteService.RemoveFavoriteActivityAsync(id);

            return result.Match(
               u => NoContent(),
               err => err.Response()
               );
        }

        [HttpPost("{activity-id}")]
        [IdValidation]
        public async Task<IActionResult> CreateFavoriteActivity(int activityId)
        {
            var result = await _favoriteService.AddFavoriteActivityAsync(activityId);

            return result.Match(
               activity => CreatedAtRoute(nameof(GetFavoriteActivity), new { activity = activity.Id }, activity),
               err => err.Response()
               );
        }
    }
}
