using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Errors;
using Application.InfrastructureInterfaces.Security;
using Application.Models.Activity;
using AutoMapper;
using DAL;
using DAL.Query;
using Domain;
using LanguageExt;

namespace Application.ServiceInterfaces
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IUserAccessor _userAccessor;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public FavoriteService(IMapper mapper, IUserAccessor userAccessor, IUnitOfWork uow)
        {
            _mapper = mapper;
            _userAccessor = userAccessor;
            _uow = uow;
        }

        public async Task<UserFavoriteActivityReturn> GetFavoriteActivityAsync(int id)
        {
            var favoredActivity = await _uow.UserFavorites.GetAsync(id);
            return _mapper.Map<UserFavoriteActivityReturn>(favoredActivity);
        }


        public async Task<IList<FavoriteActivityIdReturn>> GetAllOwnerFavoriteIdsAsync()
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();

            var favoriteActivities = await _uow.UserFavorites.GetFavoriteActivitiesAsync(userId);

            return _mapper.Map<List<FavoriteActivityIdReturn>>(favoriteActivities);
        }

        public async Task<Either<RestError, Unit>> RemoveFavoriteActivityAsync(int activityId)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();
            var existingFavoriteActivity = await _uow.UserFavorites.GetFavoriteActivityAsync(userId, activityId);

            if (existingFavoriteActivity == null)
                return new NotFound("Aktivnost nije pronadjena");

            _uow.UserFavorites.Remove(existingFavoriteActivity);
            await _uow.CompleteAsync();

            return Unit.Default;
        }

        public async Task<Either<RestError, UserFavoriteActivityReturn>> AddFavoriteActivityAsync(int activityId)
        {
            var userId = _userAccessor.GetUserIdFromAccessToken();
            var existingFavoriteActivity = await _uow.UserFavorites.GetFavoriteActivityAsync(userId, activityId);

            if (existingFavoriteActivity != null)
                return new BadRequest("Aktivnost je već omiljena");

            var userFavoriteActivity = new UserFavoriteActivity() { ActivityId = activityId, UserId = userId };

            _uow.UserFavorites.Add(userFavoriteActivity);
            await _uow.CompleteAsync();

            return _mapper.Map<UserFavoriteActivityReturn>(userFavoriteActivity);
        }

        public async Task<FavoritedActivityEnvelope> GetFavoritedActivitiesByUserAsync(int userId, ActivityQuery activityQuery)
        {
            var activities = await _uow.Activities.GetFavoritedActivitiesByUser(userId, activityQuery);

            return new FavoritedActivityEnvelope
            {
                Activities = _mapper.Map<IEnumerable<Activity>, IEnumerable<FavoritedActivityReturn>>(activities).ToList(),
                ActivityCount = await _uow.Activities.CountFavoritedActivitiesByUser(userId),
            };
        }
    }
}
