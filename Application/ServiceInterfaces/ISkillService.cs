using System.Threading.Tasks;
using Application.Errors;
using Application.Models;
using Application.Models.User;
using LanguageExt;

namespace Application.ServiceInterfaces
{
    public interface ISkillService
    {
        Task<Either<RestError, SkillData>> GetSkillsDataAsync(int userId);
        Task<Either<RestError, UserBaseResponse>> UpdateSkillsDataAsync(SkillData skillData);
    }
}
