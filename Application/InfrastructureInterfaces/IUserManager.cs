using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Identity;

namespace Application.InfrastructureInterfaces
{
    public interface IUserManager
    {
        Task<bool> CreateUserAsync(User user, string password);
        Task<bool> CreateUserWithoutPasswordAsync(User user);
        Task<User> FindUserByIdAsync(int userId);
        Task<User> FindUserByNameAsync(string name);
        Task<User> FindUserByEmailAsync(string email);
        Task<string> GenerateUserEmailConfirmationTokenAsync(User user);
        Task<string> GenerateUserPasswordResetTokenAsync(User user);
        Task<IdentityResult> RecoverUserPasswordAsync(User user, string resetToken, string newPassword);
        Task<IdentityResult> ChangeUserPasswordAsync(User user, string oldPassword, string newPassword);
        Task<bool> ConfirmUserEmailAsync(User user, string token);
        Task<SignInResult> SignInUserViaPasswordNoLockoutAsync(User user, string password);
        Task<SignInResult> SignInUserViaPasswordWithLockoutAsync(User user, string password);
        Task<bool> UpdateUserAsync(User user);
        Task SignOutUserAsync();
    }
}
