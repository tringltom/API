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
        Task<Either<RestError, int>> AnswerToPuzzleAsync(int id, PuzzleAnswer puzzleAnswer);
        Task<ApprovedActivityEnvelope> GetActivitiesFromOtherUsersAsync(ActivityQuery activityQuery);
        Task<HappeningEnvelope> GetHappeningsForApprovalAsync(QueryObject queryObject);
        Task<Either<RestError, ApprovedActivityEnvelope>> GetApprovedActivitiesForUserAsync(UserQuery userQuery);
        Task<Either<RestError, ApprovedActivityReturn>> ApprovePendingActivity(int id);
        Task<Either<RestError, Unit>> AttendToHappeningAsync(int id, bool attend);
        Task<Either<RestError, Unit>> ConfirmAttendenceToHappeningAsync(int id);
        Task<Either<RestError, Unit>> CompleteHappeningAsync(int id, HappeningUpdate happeningUpdate);
        Task<Either<RestError, Unit>> ApproveHappeningCompletitionAsync(int id, bool approve);
    }
}
