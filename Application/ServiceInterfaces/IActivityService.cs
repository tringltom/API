using System.Threading.Tasks;
using Domain.Entities;
using Models.Activity;

namespace Application.ServiceInterfaces
{
    public interface IActivityService
    {
        Task CreatePendingActivityAsync(ActivityCreate user);
        Task<ActivityEnvelope> GetPendingActivitiesAsync(int? limit, int? offset);
        Task<bool> ReslovePendingActivityAsync(int pendingActivityID, PendingActivityApproval approval);
        Task<Activity> GetActivitityUserIdByActivityId(int activityId);
        Task<ActivityEnvelope> GetApprovedActivitiesExcludingUserAsync(int userId, int? limit, int? offset);
    }
}
