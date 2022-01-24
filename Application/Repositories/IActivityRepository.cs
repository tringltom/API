using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Repositories
{
    public interface IActivityRepository
    {
        Task CreatActivityAsync(Activity activity);
        Task CreatePendingActivityAsync(PendingActivity activity);
        Task CreateActivityCreationCounter(ActivityCreationCounter activityCreationCounter);
        Task<List<PendingActivity>> GetPendingActivitiesAsync(int? limit, int? offset);
        Task<PendingActivity> GetPendingActivityByIDAsync(int id);
        Task<int> GetPendingActivitiesCountAsync();
        Task<bool> DeletePendingActivity(PendingActivity pendingActivity);
        Task<bool> DeleteActivityCountersAsync(List<ActivityCreationCounter> activityCounters);
    }
}
