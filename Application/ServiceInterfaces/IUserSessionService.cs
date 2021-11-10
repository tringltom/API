using System.Threading;
using System.Threading.Tasks;
using Models.User;

namespace Application.ServiceInterfaces
{
    public interface IUserSessionService
    {
        Task<UserBaseResponse> LoginAsync(UserLogin userLogin);
        Task<UserBaseResponse> RefreshTokenAsync(string refreshToken);
        Task<UserBaseResponse> FacebookLoginAsync(string accessToken, CancellationToken cancellationToken);
        Task<UserCurrentlyLoggedIn> GetCurrentlyLoggedInUserAsync(bool stayLoggedIn, string refreshToken);
        Task LogoutUserAsync(string refreshToken);
    }
}
