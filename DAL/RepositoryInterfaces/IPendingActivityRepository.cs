using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface IPendingActivityRepository : IBaseRepository<PendingActivity>
    {
        Task<IEnumerable<PendingActivity>> GetLatestPendingActivitiesAsync(int? limit, int? offset);
        Task<IEnumerable<PendingActivity>> GetLatestPendingActivitiesAsync(int userId, int? limit, int? offset);
        Task<int> CountPendingActivitiesAsync(int userId);
    }
}
