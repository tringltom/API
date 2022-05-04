using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.RepositoryInterfaces;
using Domain;
using Persistence;

namespace DAL.Repositories
{
    public class UserReviewRepository : BaseRepository<UserReview>, IUserReviewRepository
    {
        public UserReviewRepository(DataContext context) : base(context) { }

        public async Task<UserReview> GetUserReviewAsync(int activityId, int userId) => await GetAsync(ur => ur.ActivityId == activityId && ur.UserId == userId);

        public async Task<IEnumerable<UserReview>> GetUserReviewsAsync(int userId) => await FindAsync(ur => ur.UserId == userId);

    }
}
