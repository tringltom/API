using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface IUserReviewRepository
    {
        Task<UserReview> GetUserReviewByActivityAndUserIdAsync(int activityId, int userId);
        Task ReviewUserActivityAsync(UserReview userReview);
        Task UpdateUserActivityReviewAsync(UserReview userReview);
        IQueryable<UserReview> GetAllUserReviewsAsQeuriable();
    }
}
