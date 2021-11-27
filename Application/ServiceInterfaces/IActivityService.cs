using System.Threading.Tasks;
using Models.Activity;

namespace Application.ServiceInterfaces
{
    public interface IActivityService
    {
        Task CreateActivityAsync(ActivityCreate user);
        Task<PendingActivityEnvelope> GetPendingActivitiesAsync(int? limit, int? offset);
        Task<bool> ReslovePendingActivityAsync(int pendingActivityID, bool approve);
    }
}
