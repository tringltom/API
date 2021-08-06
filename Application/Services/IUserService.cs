using System.Threading;
using System.Threading.Tasks;
using Application.Models;
using Domain.Entities;

namespace Application.Services
{
    public interface IUserService
    {
        Task RegisterAsync(User user, string password, string origin);
        Task ResendConfirmationEmailAsync(string email, string origin);
        Task ConfirmEmailAsync(string email, string token);
        Task RecoverUserPasswordViaEmailAsync(string email, string origin);
        Task<User> ConfirmUserPasswordRecoveryAsync(string email, string token, string newPassword);
        Task ChangeUserPasswordAsync(string email, string oldPassword, string newPassword);
        Task<UserBaseServiceResponse> LoginAsync(string email, string password);
        Task<UserBaseServiceResponse> RefreshTokenAsync(string refreshToken);
        Task<UserBaseServiceResponse> FacebookLoginAsync(string accessToken, CancellationToken cancellationToken);
    }
}
