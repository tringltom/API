using DAL.RepositoryInterfaces;
using Domain;
using Persistence;

namespace DAL.Repositories
{
    public class HappeningMediaRepository : BaseRepository<HappeningMedia>, IHappeningMediaRepository
    {
        public HappeningMediaRepository(DataContext dbContext) : base(dbContext)
        {
        }
    }
}
