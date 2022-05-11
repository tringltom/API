using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Query;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface IPendingActivityRepository : IBaseRepository<PendingActivity>
    {
        Task<IEnumerable<PendingActivity>> GetLatestPendingActivitiesAsync(QueryObject queryObject);
        Task<IEnumerable<PendingActivity>> GetLatestPendingActivitiesAsync(int userId, ActivityQuery activityQuery);
        Task<int> CountPendingActivitiesAsync(int userId, ActivityQuery activityQuery);
    }
}
