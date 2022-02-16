using System.Linq;
using System.Threading.Tasks;
using DAL.RepositoryInterfaces;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Z.EntityFramework.Plus;

namespace DAL.Repositories
{
    public class UserReviewRepository : IUserReviewRepository
    {
        private readonly DataContext _context;
        public UserReviewRepository(DataContext context)
        {
            _context = context;
        }

        public IQueryable<UserReview> GetAllUserReviewsAsQeuriable()
        {
            return _context.UserReviews.AsQueryable();
        }

        private DbSet<UserReview> GetAllUserReviews()
        {
            return _context.UserReviews;
        }

        public async Task<UserReview> GetUserReviewByActivityAndUserIdAsync(int activityId, int userId)
        {
            return await GetAllUserReviews().FirstOrDefaultAsync(ur => ur.ActivityId == activityId && ur.UserId == userId);
        }

        public async Task ReviewUserActivityAsync(UserReview userReview)
        {
            GetAllUserReviews().Add(userReview);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserActivityReviewAsync(UserReview userReview)
        {
            await GetAllUserReviewsAsQeuriable().Where(x => x.UserId == userReview.UserId && x.ActivityId == userReview.ActivityId)
                   .UpdateAsync(x => new UserReview() { ReviewTypeId = userReview.ReviewTypeId });
        }
    }
}
