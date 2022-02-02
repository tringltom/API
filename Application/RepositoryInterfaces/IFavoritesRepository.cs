using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.RepositoryInterfaces
{
    public interface IFavoritesRepository
    {
        Task AddFavoriteActivityAsync(UserFavoriteActivity userFavoriteActivity);
        Task<UserFavoriteActivity> GetFavoriteActivityByIdAsync(int id);
        Task<IList<UserFavoriteActivity>> GetFavoriteActivitiesByUserIdAsync(int id);
        Task<UserFavoriteActivity> GetFavoriteActivityAsync(UserFavoriteActivity activity);
        Task<bool> RemoveFavoriteActivityByActivityAndUserIdAsync(int userId, int activityId);
    }
}
