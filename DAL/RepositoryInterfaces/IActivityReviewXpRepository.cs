using System.Threading.Tasks;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface IActivityReviewXpRepository
    {
        Task<int> GetXpRewardByActivityAndReviewTypeIdsAsync(ActivityTypeId activityTypeId, ReviewTypeId reviewTypeId);
    }
}
