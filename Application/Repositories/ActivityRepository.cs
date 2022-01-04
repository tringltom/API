using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.RepositoryInterfaces;
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

        public async Task CreatActivityAsync(Activity activity)
        {
            await _context.Activities.AddAsync(activity);
            _context.SaveChanges();
        }

        public async Task CreatePendingActivityAsync(PendingActivity activity)
        {
            _context.PendingActivities.Add(activity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeletePendingActivity(PendingActivity pendingActivity)
        {
            _context.PendingActivities.Remove(pendingActivity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<PendingActivity>> GetPendingActivitiesAsync(int? limit, int? offset)
        {
            return await _context.PendingActivities
                .AsQueryable()
                .Skip(offset ?? 0)
                .Take(limit ?? 3)
                .ToListAsync();
        }

        public async Task<int> GetPendingActivitiesCountAsync()
        {
            return await _context.PendingActivities.CountAsync();
        }

        public async Task<PendingActivity> GetPendingActivityByIDAsync(int id) => await _context.PendingActivities.FindAsync(id);

        public async Task<int> GetTypeOfActivityAsync(int activityId)
        {
            return (int)(await _context.Activities.FirstOrDefaultAsync(x => x.Id == activityId)).ActivityTypeId;
        }

        public async Task<Activity> GetActivityByIdAsync(int activityId)
        {
            return await _context.Activities.FirstOrDefaultAsync(x => x.Id == activityId);
        }
    }
}
