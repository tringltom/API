using System.Threading.Tasks;
using Application.Errors;
using Application.Models.Activity;
using LanguageExt;

namespace Application.ServiceInterfaces
{
    public interface IActivityService
    {
        Task<ApprovedActivityReturn> GetActivityAsync(int id);
        Task<ApprovedActivityEnvelope> GetActivitiesFromOtherUsersAsync(int? limit, int? offset);
        Task<Either<RestError, int>> AnswerToPuzzleAsync(int id, PuzzleAnswer puzzleAnswer);
        Task<Either<RestError, ApprovedActivityReturn>> ApprovePendingActivity(int id);
    }
}
