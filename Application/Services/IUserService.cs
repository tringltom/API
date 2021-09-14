using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Models.User;

namespace Application.Services
{
    public interface IUserService
    {
        Task RegisterAsync(User user, string password, string origin);
        Task ResendConfirmationEmailAsync(string email, string origin);
        Task ConfirmEmailAsync(string email, string token);
        Task RecoverUserPasswordViaEmailAsync(string email, string origin);
        Task ConfirmUserPasswordRecoveryAsync(string email, string token, string newPassword);
        Task ChangeUserPasswordAsync(string email, string oldPassword, string newPassword);
        Task<UserBaseResponse> LoginAsync(string email, string password);
        Task<UserBaseResponse> RefreshTokenAsync(string refreshToken);
        Task<UserBaseResponse> FacebookLoginAsync(string accessToken, CancellationToken cancellationToken);
        Task<UserCurrentlyLoggedIn> GetCurrentlyLoggedInUserAsync();
        Task LogoutUserAsync(string refreshToken);
    }
}
