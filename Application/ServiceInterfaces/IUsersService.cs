using System.Threading.Tasks;
using Application.Errors;
using Application.Models.User;
using LanguageExt;

namespace Application.ServiceInterfaces
{
    public interface IUsersService
    {
        Task<UserRankedEnvelope> GetRankedUsersAsync(int? limit, int? offset);
        Task UpdateLoggedUserAboutAsync(UserAbout userAbout);
        Task<Either<RestError, Unit>> UpdateLoggedUserImageAsync(UserImageUpdate userImage);
        Task<UserImageEnvelope> GetImagesForApprovalAsync(int? limit, int? offset);
        Task<Either<RestError, Unit>> ResolveUserImageAsync(int userId, bool approve);
    }
}
