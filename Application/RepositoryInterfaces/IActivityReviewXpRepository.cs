using System.Threading.Tasks;
using Domain.Entities;

namespace Application.RepositoryInterfaces
{
    public interface IActivityReviewXpRepository
    {
        Task<int> GetXpRewardByActivityAndReviewTypeIdsAsync(ActivityTypeId activityTypeId, ReviewTypeId reviewTypeId);
    }
}
