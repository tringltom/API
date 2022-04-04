using System.Threading.Tasks;
using DAL.RepositoryInterfaces;
using Domain;
using Persistence;

namespace DAL.Repositories
{
    public class SkillSpecialRepostiory : BaseRepository<SkillSpecial>, ISkillSpecialRepository
    {
        public SkillSpecialRepostiory(DataContext dbContext) : base(dbContext)
        {
        }

        public async Task<SkillSpecial> GetSkillSpecial(ActivityTypeId? firstActivityTypeId, ActivityTypeId? secondActivityTypeId)
        {
            return await GetAsync(ss => ss.ActivityTypeOneId == firstActivityTypeId && ss.ActivityTypeTwoId == secondActivityTypeId);
        }
    }
}
