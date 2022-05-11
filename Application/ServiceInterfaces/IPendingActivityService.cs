using System.Threading.Tasks;
using Application.Errors;
using Application.Models.Activity;
using DAL.Query;
using LanguageExt;

namespace Application.ServiceInterfaces
{
    public interface IPendingActivityService
    {
        Task<PendingActivityEnvelope> GetPendingActivitiesAsync(QueryObject queryObject);
        Task<PendingActivityForUserEnvelope> GetOwnerPendingActivitiesAsync(ActivityQuery activityQuery);
        Task<Either<RestError, ActivityCreate>> GetOwnerPendingActivityAsync(int id);
        Task<Either<RestError, PendingActivityReturn>> UpdatePendingActivityAsync(int id, ActivityCreate updatedActivityCreate);
        Task<Either<RestError, Unit>> DisapprovePendingActivityAsync(int id);
        Task<Either<RestError, PendingActivityReturn>> CreatePendingActivityAsync(ActivityCreate user);

    }
}
