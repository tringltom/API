using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userIdentityManager;

        public UserRepository(UserManager<User> userIdentityManager)
        {
            _userIdentityManager = userIdentityManager;
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

    }
}
