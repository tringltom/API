using System.Threading.Tasks;
using Application.Models;
using Application.Models.User;

namespace Application.ServiceInterfaces
{
    public interface ISkillService
    {
        Task<SkillData> GetSkillsDataAsync(int userId);
        Task<UserBaseResponse> ResetSkillsDataAsync();
        Task<UserBaseResponse> UpdateSkillsDataAsync(SkillData skillData);
    }
}
