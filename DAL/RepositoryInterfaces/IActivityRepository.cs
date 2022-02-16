using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface IActivityRepository
    {
        Task CreateActivityAsync(Activity activity);
        Task CreatePendingActivityAsync(PendingActivity activity);
        Task CreateActivityCreationCounter(ActivityCreationCounter activityCreationCounter);
        Task<List<PendingActivity>> GetPendingActivitiesAsync(int? limit, int? offset);
        Task<PendingActivity> GetPendingActivityByIdAsync(int id);
        Task<int> GetPendingActivitiesCountAsync();
        Task<bool> DeletePendingActivity(PendingActivity pendingActivity);
        Task<bool> DeleteActivityCountersAsync(List<ActivityCreationCounter> activityCounters);
        Task<Activity> GetActivityByIdAsync(int activityId);
        IQueryable<Activity> GetApprovedActivitiesAsQueriable();
        Task<int> GetApprovedActivitiesCountAsync();
    }
}
