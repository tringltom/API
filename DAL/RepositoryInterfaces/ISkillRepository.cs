using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface ISkillRepository : IBaseRepository<Skill>
    {
        Task<IEnumerable<Skill>> GetSkillsAsync(int userId);
        Task<Skill> GetSkillAsync(int userId, ActivityTypeId activityTypeId);
        Task<Skill> GetPuzzleSkillAsync(int userId);
        Task<Skill> GetHappeningSkillAsync(int userId);
    }
}
