using System.Threading.Tasks;
using Application.Models.User;

namespace Application.ServiceInterfaces
{
    public interface IUserSessionService
    {
        Task<UserBaseResponse> LoginAsync(UserLogin userLogin);
        Task<UserRefreshResponse> RefreshTokenAsync(string refreshToken);
        //Task<UserBaseResponse> FacebookLoginAsync(string accessToken, CancellationToken cancellationToken);
        Task<UserBaseResponse> GetCurrentlyLoggedInUserAsync(bool stayLoggedIn, string refreshToken);
        Task LogoutUserAsync(string refreshToken);
    }
}
