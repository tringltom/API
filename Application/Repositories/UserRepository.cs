﻿using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userIdentityManager;
        private readonly SignInManager<User> _userSigninManager;

        private readonly DataContext _context;

        public UserRepository(UserManager<User> userIdentityManager, DataContext context,
            SignInManager<User> userSigninManager)
        {
            _userIdentityManager = userIdentityManager;
            _userSigninManager = userSigninManager;
            _context = context;
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

        public async Task<string> GenerateUserEmailConfirmationTokenAsyn(User user)
        {
            return await _userIdentityManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<string> GenerateUserPasswordResetTokenAsync(User user)
        {
            return await _userIdentityManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<bool> ExistsWithEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email);
        }

        public async Task<bool> ExistsWithUsernameAsync(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username);
        }

        public async Task<SignInResult> SignInUserViaPasswordNoLockoutAsync(User user, string password)
        {
            return await _userSigninManager.CheckPasswordSignInAsync(user, password, false);
        }

        public async Task<SignInResult> SignInUserViaPasswordWithLockoutAsync(User user, string password)
        {
            return await _userSigninManager.CheckPasswordSignInAsync(user, password, true);
        }
    }
}