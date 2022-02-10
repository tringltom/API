
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.RepositoryInterfaces
{
    public interface IUserRepository
    {
        Task<bool> CreateUserAsync(User user, string password);
        Task<bool> CreateUserWithoutPasswordAsync(User user);
        Task<User> FindUserByNameAsync(string name);
        Task<User> FindUserByEmailAsync(string email);
        Task<string> GenerateUserEmailConfirmationTokenAsync(User user);
        Task<string> GenerateUserPasswordResetTokenAsync(User user);
        Task<IdentityResult> RecoverUserPasswordAsync(User user, string resetToken, string newPassword);
        Task<IdentityResult> ChangeUserPasswordAsync(User user, string oldPassword, string newPassword);
        Task<bool> ExistsWithEmailAsync(string email);
        Task<bool> ExistsWithUsernameAsync(string username);
        Task<bool> ConfirmUserEmailAsync(User user, string token);
        Task<SignInResult> SignInUserViaPasswordNoLockoutAsync(User user, string password);
        Task<SignInResult> SignInUserViaPasswordWithLockoutAsync(User user, string password);
        Task<bool> UpdateUserAsync(User user);
        Task SignOutUserAsync();
        string GetCurrentUsername();
        int GetUserIdUsingToken();
        Task<User> GetUserByUserNameAsync(string userName);
        Task<User> GetUserUsingTokenAsync();
        Task<User> GetUserByIdAsync(int userId);
        Task<RefreshToken> GetOldRefreshToken(string refreshToken);
    }
}
