using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Query;
using DAL.RepositoryInterfaces;
using Domain;
using Persistence;

namespace DAL.Repositories
{
    public class PendingActivityRepository : BaseRepository<PendingActivity>, IPendingActivityRepository
    {
        public PendingActivityRepository(DataContext dbContext) : base(dbContext) { }
        public async Task<IEnumerable<PendingActivity>> GetLatestPendingActivitiesAsync(QueryObject queryObject)
        {
            return await FindAsync(queryObject.Limit,
                queryObject.Offset,
                u => true,
                u => u.Id);
        }
        public async Task<IEnumerable<PendingActivity>> GetLatestPendingActivitiesAsync(int userId, ActivityQuery activityQuery)
        {
            return await FindAsync(activityQuery.Limit,
                 activityQuery.Offset,
                 u => u.User.Id == userId
                    && (string.IsNullOrEmpty(activityQuery.Title) || u.Title.Contains(activityQuery.Title))
                    && (activityQuery.ActivityTypes == null || activityQuery.ActivityTypes.Contains(u.ActivityTypeId)),
                 u => u.Id);
        }
        public async Task<int> CountPendingActivitiesAsync(int userId, ActivityQuery activityQuery)
        {
            return await CountAsync(pa =>
                pa.User.Id == userId
                && (string.IsNullOrEmpty(activityQuery.Title) || pa.Title.Contains(activityQuery.Title))
                && (activityQuery.ActivityTypes == null || activityQuery.ActivityTypes.Contains(pa.ActivityTypeId)));
        }
    }
}
