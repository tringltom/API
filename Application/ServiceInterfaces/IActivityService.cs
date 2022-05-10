namespace Application.ServiceInterfaces
{
    public interface IActivityService
    {
        Task<ApprovedActivityReturn> GetActivityAsync(int id);
        Task<Either<RestError, int>> AnswerToPuzzleAsync(int id, PuzzleAnswer puzzleAnswer);
        Task<ApprovedActivityEnvelope> GetActivitiesFromOtherUsersAsync(ActivityQuery activityQuery);
        Task<Either<RestError, ApprovedActivityReturn>> ApprovePendingActivity(int id);
    }
}
