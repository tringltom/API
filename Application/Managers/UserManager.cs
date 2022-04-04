using System.Threading.Tasks;
using Application.ManagerInterfaces;
using Domain;
using Microsoft.AspNetCore.Identity;

namespace Application.Managers
{
    public class UserManager : IUserManager
    {
        private readonly UserManager<User> _userIdentityManager;
        private readonly SignInManager<User> _userSigninManager;

        public UserManager(UserManager<User> userIdentityManager, SignInManager<User> userSigninManager)
        {
            _userIdentityManager = userIdentityManager;
            _userSigninManager = userSigninManager;
        }

        public async Task<bool> CreateUserAsync(User user, string password)
        {
            var result = await _userIdentityManager.CreateAsync(user, password);
            return result.Succeeded;
        }

        public async Task<bool> CreateUserWithoutPasswordAsync(User user)
        {
            var result = await _userIdentityManager.CreateAsync(user);
            return result.Succeeded;
        }
        public async Task<User> FindUserByIdAsync(int userId)
        {
            return await _userIdentityManager.FindByIdAsync(userId.ToString());
        }

        public async Task<User> FindUserByNameAsync(string name)
        {
            return await _userIdentityManager.FindByNameAsync(name);
        }

        public async Task<User> FindUserByEmailAsync(string email)
        {
            return await _userIdentityManager.FindByEmailAsync(email);
        }


        public async Task<bool> ConfirmUserEmailAsync(User user, string token)
        {
            var result = await _userIdentityManager.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }

        public async Task<IdentityResult> RecoverUserPasswordAsync(User user, string resetToken, string newPassword)
        {
            return await _userIdentityManager.ResetPasswordAsync(user, resetToken, newPassword);
        }

        public async Task<IdentityResult> ChangeUserPasswordAsync(User user, string oldPassword, string newPassword)
        {
            return await _userIdentityManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var result = await _userIdentityManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<string> GenerateUserEmailConfirmationTokenAsync(User user)
        {
            return await _userIdentityManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<string> GenerateUserPasswordResetTokenAsync(User user)
        {
            return await _userIdentityManager.GeneratePasswordResetTokenAsync(user);
        }
        public async Task<SignInResult> SignInUserViaPasswordNoLockoutAsync(User user, string password)
        {
            return await _userSigninManager.CheckPasswordSignInAsync(user, password, false);
        }

        public async Task<SignInResult> SignInUserViaPasswordWithLockoutAsync(User user, string password)
        {
            return await _userSigninManager.CheckPasswordSignInAsync(user, password, true);
        }

        public async Task SignOutUserAsync()
        {
            await _userSigninManager.SignOutAsync();
        }
    }
}
