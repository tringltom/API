using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface IPendingActivityRepository : IBaseRepository<PendingActivity>
    {
        Task<IEnumerable<PendingActivity>> GetLatestPendingActivities(int? limit, int? offset);
        Task<IEnumerable<PendingActivity>> GetLatestPendingActivities(int userId, int? limit, int? offset);
        Task<int> CountPendingActivities(int userId);
    }
}
