using DAL.RepositoryInterfaces;
using Domain;
using Persistence;

namespace DAL.Repositories
{
    public class ActivityCreationCounterRepository : BaseRepository<ActivityCreationCounter>, IActivityCreationCounterRepository
    {
        public ActivityCreationCounterRepository(DataContext dbContext) : base(dbContext) { }
    }
}
