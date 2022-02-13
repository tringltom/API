using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Application.Errors;
using Application.RepositoryInterfaces;
using AutoMapper;
using Domain.Entities;
using Models.Activity;

namespace Application.ServiceInterfaces
{
    public class FavoritesService : IFavoritesService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFavoritesRepository _favoritesRepository;
        private readonly IMapper _mapper;

        public FavoritesService(IFavoritesRepository favoritesRepository, IMapper mapper, IUserRepository userRepository)
        {
            _favoritesRepository = favoritesRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task ResolveFavoriteActivityAsync(FavoriteActivityBase activity)
        {
            var userId = _userRepository.GetUserIdUsingToken();

            var userFavoriteActivity = new UserFavoriteActivity() { ActivityId = activity.ActivityId, UserId = userId };

            try
            {
                if (activity.Favorite)
                {
                    await _favoritesRepository.AddFavoriteActivityAsync(userFavoriteActivity);
                }
                else
                {
                    if (!await _favoritesRepository.RemoveFavoriteActivityByActivityAndUserIdAsync(userId, activity.ActivityId))
                        throw new RestException(HttpStatusCode.BadRequest, new { FavoriteActivity = "Greška, aktivnost je nepostojeća." });
                }
            }
            catch (Exception)
            {
                throw new RestException(HttpStatusCode.BadRequest, new { FavoriteActivity = "Greška, korisnik i/ili aktivnost su nepostojeći." });
            }
        }

        public async Task<IList<FavoriteActivityReturn>> GetAllFavoritesForUserAsync(int userId)
        {
            var favoriteActivities = await _favoritesRepository.GetFavoriteActivitiesByUserIdAsync(userId);

            return _mapper.Map<List<FavoriteActivityReturn>>(favoriteActivities);
        }
    }
}
