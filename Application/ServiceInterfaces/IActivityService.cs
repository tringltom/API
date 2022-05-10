using System.Threading.Tasks;
using Application.Errors;
using Application.Models.Activity;
using DAL.Query;
using LanguageExt;

namespace Application.ServiceInterfaces
{
    public interface IActivityService
    {
        Task<ApprovedActivityReturn> GetActivityAsync(int id);
        Task<ApprovedActivityEnvelope> GetActivitiesFromOtherUsersAsync(ActivityQuery activityQuery);
        Task<Either<RestError, ApprovedActivityReturn>> ApprovePendingActivity(int id);
    }
}
