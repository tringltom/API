using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.RepositoryInterfaces;
using Domain;
using Persistence;

namespace DAL.Repositories
{
    public class SkillRepository : BaseRepository<Skill>, ISkillRepository
    {
        public SkillRepository(DataContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<Skill>> GetSkillsAsync(int userId) => await FindAsync(s => s.User.Id == userId);
        public async Task<Skill> GetSkillAsync(int userId, ActivityTypeId activityTypeId) => await GetAsync(s => s.User.Id == userId && s.ActivityTypeId == activityTypeId);
        public async Task<Skill> GetPuzzleSkillAsync(int userId) => await GetAsync(s => s.User.Id == userId && s.ActivityTypeId == ActivityTypeId.Puzzle);
    }
}
