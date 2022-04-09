using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.RepositoryInterfaces;
using Domain;
using Persistence;

namespace DAL.Repositories
{
    public class ActivityRepository : BaseRepository<Activity>, IActivityRepository
    {
        public ActivityRepository(DataContext context) : base(context) { }

        public async Task<IEnumerable<Activity>> GetOrderedActivitiesFromOtherUsersAsync(int? limit, int? offset, int userId)
        {
            return await FindAsync(limit, offset, a => a.User.Id != userId, a => a.Id);
        }

        public async Task<int> CountOtherUsersActivitiesAsync(int userId)
        {
            return await CountAsync(a => a.User.Id != userId);
        }
    }
}
