using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Query;
using DAL.RepositoryInterfaces;
using Domain;
using Persistence;

namespace DAL.Repositories
{
    internal class UserChallengeAnswerRepository : BaseRepository<UserChallengeAnswer>, IUserChallengeAnswerRepostiory
    {
        public UserChallengeAnswerRepository(DataContext dbContext) : base(dbContext) { }

        public async Task<UserChallengeAnswer> GetUserChallengeAnswerAsync(int userId, int activityId)
        {
            return await GetAsync(uc => uc.UserId == userId && uc.ActivityId == activityId);
        }

        public async Task<IEnumerable<UserChallengeAnswer>> GetUserChallengeAnswersAsync(int activityId, QueryObject queryObject)
        {
            return await FindAsync(queryObject.Limit,
               queryObject.Offset,
               uc => uc.ActivityId == activityId,
               uc => uc.Id);
        }

        public async Task<IEnumerable<UserChallengeAnswer>> GetNotConfirmedUserChallengeAnswersAsync(int activityId)
        {
            return await FindAsync(uc => uc.ActivityId == activityId && !uc.Confirmed);
        }

        public async Task<UserChallengeAnswer> GetConfirmedUserChallengeAnswersAsync(int activityId)
        {
            return await GetAsync(uc => uc.ActivityId == activityId && uc.Confirmed);
        }

        public async Task<int> CountChallengeAnswersAsync(int activityId)
        {
            return await CountAsync(ca => ca.ActivityId == activityId);
        }
    }
}
