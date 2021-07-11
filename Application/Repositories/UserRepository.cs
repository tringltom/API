using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userIdentityManager;
        private readonly DataContext _context;

        public UserRepository(UserManager<User> userIdentityManager, DataContext context)
        {
            _userIdentityManager = userIdentityManager;
            _context = context;
        }

        public async Task<bool> CreateUserAsync(User user, string password)
        {
            var result = await _userIdentityManager.CreateAsync(user, password);
            return result.Succeeded;
        }

        public async Task<User> FindUserByEmailAsync(string email)
        {
            return await _userIdentityManager.FindByEmailAsync(email);
        }

        public async Task<bool> ExistsWithEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email);
        }

        public async Task<bool> ExistsWithUsernameAsync(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username);
        }
    }
}
