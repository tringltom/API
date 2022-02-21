using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface IUserFavoritesRepository : IBaseRepository<UserFavoriteActivity>
    {
        Task<IEnumerable<UserFavoriteActivity>> GetFavoriteActivitiesAsync(int userId);
        Task<UserFavoriteActivity> GetFavoriteActivityAsync(int userId, int activityId);
    }
}
