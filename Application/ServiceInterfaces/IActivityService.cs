using System.Threading.Tasks;
using Application.Errors;
using Application.Models.Activity;
using LanguageExt;

namespace Application.ServiceInterfaces
{
    public interface IActivityService
    {
        Task CreatePendingActivityAsync(ActivityCreate user);
        Task<PendingActivityEnvelope> GetPendingActivitiesAsync(int? limit, int? offset);
        Task<PendingActivityForUserEnvelope> GetPendingActivitiesForLoggedInUserAsync(int? limit, int? offset);
        Task<Either<RestException, ApprovedActivityReturn>> ApprovePendingActivity(int id);
        Task<bool> ReslovePendingActivityAsync(int pendingActivityID, PendingActivityApproval approval);
        Task<ApprovedActivityEnvelope> GetActivitiesFromOtherUsersAsync(int? limit, int? offset);
        Task<ApprovedActivityReturn> GetActivityAsync(int id);
    }
}
