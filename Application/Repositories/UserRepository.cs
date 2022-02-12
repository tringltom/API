using System.Collections.Generic;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.RepositoryInterfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userIdentityManager;
        private readonly SignInManager<User> _userSigninManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly DataContext _context;

        public UserRepository(UserManager<User> userIdentityManager, DataContext context,
            SignInManager<User> userSigninManager, IHttpContextAccessor httpContextAccessor)
        {
            _userIdentityManager = userIdentityManager;
            _userSigninManager = userSigninManager;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
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

        public async Task<string> GenerateUserEmailConfirmationTokenAsync(User user)
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

        public async Task SignOutUserAsync()
        {
            await _userSigninManager.SignOutAsync();
        }

        public async Task<SignInResult> SignInUserViaPasswordNoLockoutAsync(User user, string password)
        {
            return await _userSigninManager.CheckPasswordSignInAsync(user, password, false);
        }

        public async Task<SignInResult> SignInUserViaPasswordWithLockoutAsync(User user, string password)
        {
            return await _userSigninManager.CheckPasswordSignInAsync(user, password, true);
        }

        public string GetCurrentUsername()
        {
            var username = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            return username;
        }

        public int GetUserIdUsingToken()
        {
            return Convert.ToInt32(_httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid)?.Value);
        }

        public async Task<User> GetUserUsingTokenAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid)?.Value;
            return await GetUserByIdAsync(Convert.ToInt32(userId));
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userIdentityManager.FindByIdAsync(userId.ToString());
        }

        public async Task<RefreshToken> GetOldRefreshToken(string refreshToken)
        {
            return await _context.RefreshTokens.SingleOrDefaultAsync(r => r.Token == refreshToken);
        }

        public async Task<List<User>> GetTopXpUsersAsync(int? limit, int? offset)
        {
            return await _context.Users
             .AsQueryable()
             .OrderByDescending(u => u.CurrentXp)
             .Skip(offset ?? 0)
             .Take(limit ?? 3)
             .ToListAsync();
        }

        public async Task<int> GetUserCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.UserName == userName);
        }
    }
}
