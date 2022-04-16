using System.Threading.Tasks;
using Application.Errors;
using Application.Models.Activity;
using LanguageExt;

namespace Application.ServiceInterfaces
{
    public interface IPendingActivityService
    {
        Task<PendingActivityEnvelope> GetPendingActivitiesAsync(int? limit, int? offset);
        Task<PendingActivityForUserEnvelope> GetOwnerPendingActivitiesAsync(int? limit, int? offset);
        Task<Either<RestError, PendingActivityReturn>> GetOwnerPendingActivityAsync(int id);
        Task<Either<RestError, PendingActivityReturn>> UpdatePendingActivityAsync(int id, ActivityCreate updatedActivityCreate);
        Task<Either<RestError, Unit>> DisapprovePendingActivityAsync(int id);
        Task<Either<RestError, PendingActivityReturn>> CreatePendingActivityAsync(ActivityCreate user);

    }
}
