using System.Threading.Tasks;
using Domain;
using Models.Activity;

namespace Application.ServiceInterfaces
{
    public interface IActivityService
    {
        Task CreatePendingActivityAsync(ActivityCreate user);
        Task<PendingActivityEnvelope> GetPendingActivitiesAsync(int? limit, int? offset);
        Task<bool> ReslovePendingActivityAsync(int pendingActivityID, PendingActivityApproval approval);
        Task<Activity> GetActivityUserIdByActivityId(int activityId);
        Task<ApprovedActivityEnvelope> GetApprovedActivitiesFromOtherUsersAsync(int userId, int? limit, int? offset);
    }
}
