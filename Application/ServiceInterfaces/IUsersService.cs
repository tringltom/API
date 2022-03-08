using System.Threading.Tasks;
using Application.Models.User;

namespace Application.ServiceInterfaces
{
    public interface IUsersService
    {
        Task<UserRangingEnvelope> GetRangingUsers(int? limit, int? offset);
        Task UpdateLoggedUserAboutAsync(UserAbout userAbout);
        Task UpdateLoggedUserImageAsync(UserImageUpdate userImage);
        Task<UserImageEnvelope> GetImagesForApprovalAsync(int? limit, int? offset);
        Task<bool> ResolveUserImageAsync(int userId, bool approve);
    }
}
