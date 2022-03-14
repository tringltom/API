using System.Threading.Tasks;
using Application.Models;

namespace Application.ServiceInterfaces
{
    public interface ISkillService
    {
        Task<SkillData> GetSkillsDataAsync(int userId);
        Task ResetSkillsDataAsync();
        Task UpdateSkillsDataAsync(SkillData skillData);
    }
}
