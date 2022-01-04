using System.Threading.Tasks;

namespace Application.RepositoryInterfaces
{
    public interface IActivityReviewXpRepository
    {
        Task<int> GetXpRewardByActivityAndReviewTypeIdsAsync(int activityTypeId, int reviewTypeId);
    }
}
