using System.Threading.Tasks;
using Domain.Entities;

namespace Application.ServiceInterfaces
{
    public interface IUserLevelingService
    {
        Task<bool> ReviewerExistsAsync(int reviewerId);
        Task<int> GetXpRewardYieldByReviewAsync(ActivityTypeId activityTypId, ReviewTypeId reviewTypeId);
        Task UpdateUserXpAsync(int amount, int userId);
    }
}
