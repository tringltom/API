using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface IUserReviewRepository : IBaseRepository<UserReview>
    {
        Task<UserReview> GetUserReview(int activityId, int userId);
        Task<IEnumerable<UserReview>> GetUserReviews(int userId);
    }
}
