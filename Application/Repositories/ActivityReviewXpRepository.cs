using System.Linq;
using System.Threading.Tasks;
using Application.RepositoryInterfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Repositories
{
    public class ActivityReviewXpRepository : IActivityReviewXpRepository
    {
        private readonly DataContext _context;

        public ActivityReviewXpRepository(DataContext context)
        {
            _context = context;
        }

        private IQueryable<ActivityReviewXp> ActivityReviewXpAsQueriable()
        {
            return _context.ActivityReviewXp.AsQueryable();
        }

        private DbSet<ActivityReviewXp> ActivityReviewXp()
        {
            return _context.ActivityReviewXp;
        }

        public async Task<int> GetXpRewardByActivityAndReviewTypeIdsAsync(int activityTypeId, int reviewTypeId)
        {
            return (await ActivityReviewXpAsQueriable().FirstOrDefaultAsync(x => (int)x.ActivityTypeId == activityTypeId && (int)x.ReviewTypeId == reviewTypeId)).Xp;
        }
    }
}
