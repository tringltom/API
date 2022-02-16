using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces;
using Application.InfrastructureInterfaces.Security;
using AutoMapper;
using DAL.RepositoryInterfaces;
using Domain;
using Models.Activity;

namespace Application.ServiceInterfaces
{
    public class FavoritesService : IFavoritesService
    {
        private readonly IUserManager _userManager;
        private readonly IUserAccessor _userAccessor;
        private readonly IFavoritesRepository _favoritesRepository;
        private readonly IMapper _mapper;

        public FavoritesService(IFavoritesRepository favoritesRepository, IMapper mapper, IUserManager userManager, IUserAccessor userAccessor)
        {
            _favoritesRepository = favoritesRepository;
            _mapper = mapper;
            _userManager = userManager;
            _userAccessor = userAccessor;
        }

        public async Task ResolveFavoriteActivityAsync(FavoriteActivityBase activity)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();

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
