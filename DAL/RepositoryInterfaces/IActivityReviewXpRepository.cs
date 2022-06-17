using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface IActivityReviewXpRepository : IBaseRepository<ActivityReviewXp>
    {
        Task<ActivityReviewXp> GetXpRewardAsync(ActivityTypeId activityTypeId, ReviewTypeId reviewTypeId);
        Task<IEnumerable<ActivityReviewXp>> GetChallengeXpRewardAsync();
    }
}
