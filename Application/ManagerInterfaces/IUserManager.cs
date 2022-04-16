using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Identity;

namespace Application.ManagerInterfaces
{
    public interface IUserManager
    {
        Task<bool> CreateUserAsync(User user, string password);
        Task<User> FindUserByIdAsync(int userId);
        Task<User> FindUserByNameAsync(string name);
        Task<User> FindUserByEmailAsync(string email);
        Task<string> GenerateUserEmailConfirmationTokenAsync(User user);
        Task<string> GenerateUserPasswordResetTokenAsync(User user);
        Task<IdentityResult> RecoverUserPasswordAsync(User user, string resetToken, string newPassword);
        Task<bool> ConfirmUserEmailAsync(User user, string token);
        Task<SignInResult> SignInUserViaPasswordWithLockoutAsync(User user, string password);
        Task<bool> UpdateUserAsync(User user);
    }
}
