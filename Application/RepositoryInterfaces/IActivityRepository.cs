using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.RepositoryInterfaces
{
    public interface IActivityRepository
    {
        Task CreatePendingActivityAsync(PendingActivity activity);
        Task<List<PendingActivity>> GetPendingActivitiesAsync(int? limit, int? offset);
        Task<int> GetPendingActivitiesCountAsync();
        Task<PendingActivity> GetPendingActivityByIDAsync(int id);
        Task CreatActivityAsync(Activity activity);
        Task<bool> DeletePendingActivity(PendingActivity pendingActivity);
        Task<int> GetTypeOfActivityAsync(int activityId);
        Task<Activity> GetActivityByIdAsync(int activityId);
        IQueryable<Activity> GetApprovedActivitiesAsQueriable();
        Task<int> GetApprovedActivitiesCountAsync();

    }
}
