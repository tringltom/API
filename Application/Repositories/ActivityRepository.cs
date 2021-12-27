using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly DataContext _context;
        public ActivityRepository(DataContext context)
        {
            _context = context;
        }

        public async Task CreateActivityAsync(PendingActivity activity)
        {
            _context.PendingActivities.Add(activity);
            await _context.SaveChangesAsync();
        }

        public async Task<Activity> GetActivityByIdAsync(int id)
        {
            return await _context.Activities.SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}
