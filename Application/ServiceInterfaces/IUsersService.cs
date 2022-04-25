using System.Threading.Tasks;
using Application.Errors;
using Application.Models.User;
using LanguageExt;

namespace Application.ServiceInterfaces
{
    public interface IUsersService
    {
        Task<UserRankedEnvelope> GetRankedUsersAsync(int? limit, int? offset);
        Task<Unit> UpdateAboutAsync(UserAbout userAbout);
        Task<Either<RestError, Unit>> UpdateImageAsync(UserImageUpdate userImage);
        Task<UserImageEnvelope> GetImagesForApprovalAsync(int? limit, int? offset);
        Task<Either<RestError, Unit>> ResolveImageAsync(int userId, bool approve);
    }
}
