using System.Threading.Tasks;
using Application.Errors;
using Application.Models.User;
using DAL.Query;
using LanguageExt;

namespace Application.ServiceInterfaces
{
    public interface IUsersService
    {
        Task<UserRankedEnvelope> GetRankedUsersAsync(UserQuery userQuery);
        Task<Unit> UpdateAboutAsync(UserAbout userAbout);
        Task<Either<RestError, Unit>> UpdateImageAsync(UserImageUpdate userImage);
        Task<UserImageEnvelope> GetImagesForApprovalAsync(QueryObject queryObject);
        Task<Either<RestError, Unit>> ResolveImageAsync(int userId, bool approve);
    }
}
