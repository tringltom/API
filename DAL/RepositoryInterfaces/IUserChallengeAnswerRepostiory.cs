using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Query;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface IUserChallengeAnswerRepostiory : IBaseRepository<UserChallengeAnswer>
    {
        Task<UserChallengeAnswer> GetUserChallengeAnswerAsync(int userId, int activityId);
        Task<IEnumerable<UserChallengeAnswer>> GetUserChallengeAnswersAsync(int activityId, QueryObject queryObject);
        Task<IEnumerable<UserChallengeAnswer>> GetNotConfirmedUserChallengeAnswersAsync(int activityId);
        Task<UserChallengeAnswer> GetConfirmedUserChallengeAnswersAsync(int activityId);
        Task<int> CountChallengeAnswersAsync(int activityId);
    }
}
