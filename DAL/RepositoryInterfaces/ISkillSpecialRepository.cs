using System.Threading.Tasks;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface ISkillSpecialRepository : IBaseRepository<SkillSpecial>
    {
        Task<SkillSpecial> GetSkillSpecialAsync(ActivityTypeId? firstActivityTypeId, ActivityTypeId? secondActivityTypeId);
    }
}
