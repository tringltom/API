using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Errors;
using Application.Models.Activity;
using DAL.Query;
using LanguageExt;

namespace Application.ServiceInterfaces
{
    public interface IFavoriteService
    {
        Task<UserFavoriteActivityReturn> GetFavoriteActivityAsync(int id);
        Task<IList<FavoriteActivityIdReturn>> GetAllOwnerFavoriteIdsAsync();
        Task<FavoritedActivityEnvelope> GetFavoritedActivitiesByUserAsync(int userId, ActivityQuery activityQuery);
        Task<Either<RestError, Unit>> RemoveFavoriteActivityAsync(int activityId);
        Task<Either<RestError, UserFavoriteActivityReturn>> AddFavoriteActivityAsync(int activityId);
    }
}
