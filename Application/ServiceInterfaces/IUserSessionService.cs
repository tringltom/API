using System.Threading;
using System.Threading.Tasks;
using Application.Models.User;

namespace Application.ServiceInterfaces
{
    public interface IUserSessionService
    {
        Task<UserBaseResponse> LoginAsync(UserLogin userLogin);
        Task<UserBaseResponse> RefreshTokenAsync(string refreshToken);
        Task<UserBaseResponse> FacebookLoginAsync(string accessToken, CancellationToken cancellationToken);
        Task<UserBaseResponse> GetCurrentlyLoggedInUserAsync(bool stayLoggedIn, string refreshToken);
        int GetUserIdByToken();
        Task LogoutUserAsync(string refreshToken);
    }
}
