using System.Threading.Tasks;
using Domain.Entities;
using Persistence;

namespace Application.Repositories;

public class ActivityRepository : IActivityRepository
{
    private readonly DataContext _context;
    public ActivityRepository(DataContext context)
    {
        _context = context;
    }

    public async Task CreateActivityAsync(PendingActivity activity)
    {
        await _context.PendingActivities.AddAsync(activity);
        _context.SaveChanges();
    }
}
