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

        public async Task<int> GetXpRewardByActivityAndReviewTypeIdsAsync(ActivityTypeId activityTypeId, ReviewTypeId reviewTypeId)
        {
            return (await ActivityReviewXpAsQueriable().FirstOrDefaultAsync(x => x.ActivityTypeId == activityTypeId && x.ReviewTypeId == reviewTypeId)).Xp;
        }
    }
}
