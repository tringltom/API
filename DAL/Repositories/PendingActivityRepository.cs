using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.RepositoryInterfaces;
using Domain;
using Persistence;

namespace DAL.Repositories
{
    public class PendingActivityRepository : BaseRepository<PendingActivity>, IPendingActivityRepository
    {
        public PendingActivityRepository(DataContext dbContext) : base(dbContext)
        {
        }
        public async Task<IEnumerable<PendingActivity>> GetLatestPendingActivities(int? limit, int? offset) => await FindAsync(limit, offset, u => true, u => u.Id);

    }
}
