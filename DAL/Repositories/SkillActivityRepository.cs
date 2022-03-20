using DAL.RepositoryInterfaces;
using Domain;
using Persistence;

namespace DAL.Repositories
{
    public class SkillActivityRepository : BaseRepository<SkillActivity>, ISkillActivityRepository
    {
        public SkillActivityRepository(DataContext dbContext) : base(dbContext)
        {
        }
    }
}
