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
        Task<ChallengeEnvelope> GetChallengesForApprovalAsync(QueryObject queryObject);
        Task<Either<RestError, ChallengeAnswerEnvelope>> GetOwnerChallengeAnswersAsync(int id, QueryObject queryObject);
        Task<Either<RestError, ApprovedActivityReturn>> ApprovePendingActivity(int id);
        Task<Either<RestError, Unit>> AttendToHappeningAsync(int id, bool attend);
        Task<Either<RestError, Unit>> ConfirmAttendenceToHappeningAsync(int id);
        Task<Either<RestError, Unit>> ConfirmChallengeAnswerAsync(int challengeAnswerId);
        Task<Either<RestError, Unit>> CompleteHappeningAsync(int id, HappeningUpdate happeningUpdate);
        Task<Either<RestError, Unit>> ApproveHappeningCompletitionAsync(int id, bool approve);
        Task<Either<RestError, Unit>> AnswerToChallengeAsync(int id, ChallengeAnswer challengeAnswer);
        Task<Either<RestError, Unit>> DisapproveChallengeAnswerAsync(int challengeAnswerId);
        Task<Either<RestError, Unit>> ApproveChallengeAnswerAsync(int challengeAnswerId);
    }
}
