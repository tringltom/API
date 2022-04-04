using System.Threading.Tasks;
using DAL.RepositoryInterfaces;
using Domain;
using Persistence;

namespace DAL.Repositories
{
    public class SkillXpBonusRepository : BaseRepository<SkillXpBonus>, ISkillXpBonusRepository
    {
        public SkillXpBonusRepository(DataContext dbContext) : base(dbContext)
        {
        }
        public async Task<int> GetSkillMultiplier(Skill skill)
        {
            var skillXpBonus = await GetAsync(sm => sm.Level == skill.Level - 3);
            return skillXpBonus?.Multiplier ?? 1;
        }
    }
}
