using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface IUserReviewRepository : IBaseRepository<UserReview>
    {
        Task<UserReview> GetUserReviewAsync(int activityId, int userId);
        Task<IEnumerable<UserReview>> GetUserReviewsAsync(int userId);
    }
}
