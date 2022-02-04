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

        public async Task CreateActivityAsync(Activity activity)
        {
            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Activity> GetApprovedActivitiesAsQueriable()
        {
            return _context.Activities.AsQueryable();
        }

        public async Task CreatePendingActivityAsync(PendingActivity activity)
        {
            _context.PendingActivities.Add(activity);
            await _context.SaveChangesAsync();
        }

        public async Task CreateActivityCreationCounter(ActivityCreationCounter activityCreationCounter)
        {
            await _context.ActivityCreationCounters.AddAsync(activityCreationCounter);
            _context.SaveChanges();
        }

        public async Task<List<PendingActivity>> GetPendingActivitiesAsync(int? limit, int? offset)
        {
            return await _context.PendingActivities
                .AsQueryable()
                .Skip(offset ?? 0)
                .Take(limit ?? 3)
                .ToListAsync();
        }

        public async Task<int> GetPendingActivitiesCountAsync() => await _context.PendingActivities.CountAsync();

        public async Task<PendingActivity> GetPendingActivityByIdAsync(int id) => await _context.PendingActivities.FindAsync(id);

        public async Task<bool> DeletePendingActivity(PendingActivity pendingActivity)
        {
            _context.PendingActivities.Remove(pendingActivity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteActivityCountersAsync(List<ActivityCreationCounter> activityCounters)
        {
            _context.ActivityCreationCounters.RemoveRange(activityCounters);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<int> GetApprovedActivitiesCountAsync()
        {
            return await _context.Activities.CountAsync();
        }

        public async Task<PendingActivity> GetPendingActivityByIdAsync(int id) => await _context.PendingActivities.FindAsync(id);

        public async Task<Activity> GetActivityByIdAsync(int activityId)
        {
            return await _context.Activities.FirstOrDefaultAsync(x => x.Id == activityId);
        }
    }
}
