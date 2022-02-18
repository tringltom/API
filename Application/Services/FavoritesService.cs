using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces;
using Application.InfrastructureInterfaces.Security;
using Application.Models.Activity;
using AutoMapper;
using DAL;
using Domain;

namespace Application.ServiceInterfaces
{
    public class FavoritesService : IFavoritesService
    {
        private readonly IUserManager _userManager;
        private readonly IUserAccessor _userAccessor;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public FavoritesService(IMapper mapper, IUserManager userManager, IUserAccessor userAccessor, IUnitOfWork uow)
        {
            _mapper = mapper;
            _userManager = userManager;
            _userAccessor = userAccessor;
            _uow = uow;
        }

        public async Task ResolveFavoriteActivityAsync(FavoriteActivityBase activity)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();

            var userFavoriteActivity = new UserFavoriteActivity() { ActivityId = activity.ActivityId, UserId = userId };

            if (activity.Favorite)
                _uow.UserFavorites.Add(userFavoriteActivity);
            else
                _uow.UserFavorites.Remove(userFavoriteActivity);

            if (!await _uow.CompleteAsync())
                throw new RestException(HttpStatusCode.BadRequest, new { FavoriteActivity = "Greška, korisnik i/ili aktivnost su nepostojeći." });
        }

        public async Task<IList<FavoriteActivityReturn>> GetAllFavoritesForUserAsync(int userId)
        {
            var favoriteActivities = await _uow.UserFavorites.GetFavoriteActivitiesAsync(userId);

            return _mapper.Map<List<FavoriteActivityReturn>>(favoriteActivities);
        }
    }
}
