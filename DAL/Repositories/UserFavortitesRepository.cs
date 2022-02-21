using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.RepositoryInterfaces;
using Domain;
using Persistence;

namespace DAL.Repositories
{
    public class UserFavortitesRepository : BaseRepository<UserFavoriteActivity>, IUserFavoritesRepository
    {

        public UserFavortitesRepository(DataContext context) : base(context) { }

        public async Task<UserFavoriteActivity> GetFavoriteActivityAsync(int userId, int activityId)
        {
            return await GetAsync(fa => fa.UserId == userId && fa.ActivityId == activityId);
        }

        public async Task<IEnumerable<UserFavoriteActivity>> GetFavoriteActivitiesAsync(int userId)
        {
            return await FindAsync(fa => fa.User.Id == userId);
        }
    }
}
