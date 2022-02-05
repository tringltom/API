using System.Threading.Tasks;
using Domain.Entities;

namespace Application.ServiceInterfaces
{
    public interface IUserLevelingService
    {
        Task<bool> ReviewerExistsAsync(int reviewerId);
        Task<int> GetXpRewardYieldByReviewAsync(UserReview userReview);
        Task UpdateUserXpAsync(int amount, int userId);
    }
}
