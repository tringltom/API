using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Query;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface IActivityRepository : IBaseRepository<Activity>
    {
        Task<IEnumerable<Activity>> GetOrderedActivitiesFromOtherUsersAsync(ActivityQuery activityQuery, int userId);

        Task<int> CountOtherUsersActivitiesAsync(int userId, ActivityQuery activityQuery);
    }
}
