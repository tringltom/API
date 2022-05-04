using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface IActivityRepository : IBaseRepository<Activity>
    {
        Task<IEnumerable<Activity>> GetOrderedActivitiesFromOtherUsersAsync(int? limit, int? offset, int userId);

        Task<int> CountOtherUsersActivitiesAsync(int userId);
    }
}
