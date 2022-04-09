using System.Threading.Tasks;
using Application.Models.Activity;
using Domain;

namespace Application.ServiceInterfaces
{
    public interface IActivityService
    {
        Task CreatePendingActivityAsync(ActivityCreate user);
        Task<PendingActivityEnvelope> GetPendingActivitiesAsync(int? limit, int? offset);
        Task<PendingActivityForUserEnvelope> GetPendingActivitiesForLoggedInUserAsync(int? limit, int? offset);
        Task<Activity> ApprovePendingActivity(int id);
        Task<bool> ReslovePendingActivityAsync(int pendingActivityID, PendingActivityApproval approval);
        Task<ApprovedActivityEnvelope> GetActivitiesFromOtherUsersAsync(int? limit, int? offset);
        Task<Activity> GetActivityAsync(int id);
    }
}
