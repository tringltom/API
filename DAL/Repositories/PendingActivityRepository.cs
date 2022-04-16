using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.RepositoryInterfaces;
using Domain;
using Persistence;

namespace DAL.Repositories
{
    public class PendingActivityRepository : BaseRepository<PendingActivity>, IPendingActivityRepository
    {
        public PendingActivityRepository(DataContext dbContext) : base(dbContext) { }
        public async Task<IEnumerable<PendingActivity>> GetLatestPendingActivitiesAsync(int? limit, int? offset) => await FindAsync(limit, offset, u => true, u => u.Id);
        public async Task<IEnumerable<PendingActivity>> GetLatestPendingActivitiesAsync(int userId, int? limit, int? offset) => await FindAsync(limit, offset, u => u.User.Id == userId, u => u.Id);
        public async Task<int> CountPendingActivitiesAsync(int userId) => await CountAsync(pa => pa.User.Id == userId);
    }
}
