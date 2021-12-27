using System.Threading.Tasks;
using Domain.Entities;

namespace Application.RepositoryInterfaces
{
    public interface IUserReviewRepository
    {
        Task<UserReview> GetUserReviewByActivityAndUserIdAsync(int activityId, int userId);
        Task ReviewUserActivityAsync(UserReview userReview);
        Task UpdateUserActivityReviewAsync(UserReview userReview);
    }
}
