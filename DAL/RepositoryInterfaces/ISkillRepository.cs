using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface ISkillRepository : IBaseRepository<Skill>
    {
        Task<IEnumerable<Skill>> GetSkills(int userId);
        Task<Skill> GetSkill(int userId, ActivityTypeId activityTypeId);
    }
}
