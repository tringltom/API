using Application.Models;
using Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IUserService
    {
        Task RegisterAsync(User user, string password, string origin);
        Task ResendConfirmationEmailAsync(string email, string origin);
        Task ConfirmEmailAsync(string email, string token);
        Task<UserBaseServiceResponse> LoginAsync(string email, string password);
        Task<UserBaseServiceResponse> RefreshTokenAsync(string refreshToken);
        Task<UserBaseServiceResponse> FacebookLoginAsync(string accessToken, CancellationToken cancellationToken);
    }
}
