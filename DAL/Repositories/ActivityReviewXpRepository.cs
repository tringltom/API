using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.RepositoryInterfaces;
using Domain;
using Persistence;

namespace DAL.Repositories
{
    public class ActivityReviewXpRepository : BaseRepository<ActivityReviewXp>, IActivityReviewXpRepository
    {

        public ActivityReviewXpRepository(DataContext context) : base(context) { }

        public async Task<IEnumerable<ActivityReviewXp>> GetChallengeXpRewardAsync()
        {
            return await FindAsync(x => x.ActivityTypeId == ActivityTypeId.Challenge);
        }

        public async Task<ActivityReviewXp> GetXpRewardAsync(ActivityTypeId activityTypeId, ReviewTypeId reviewTypeId)
        {
            return await GetAsync(x => x.ActivityTypeId == activityTypeId && x.ReviewTypeId == reviewTypeId);
        }
    }
}
