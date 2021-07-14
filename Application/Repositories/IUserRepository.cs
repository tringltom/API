
using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IUserRepository
    {
        Task<bool> CreateUserAsync(User user, string password);
        Task<bool> CreateUserWithoutPasswordAsync(User user);
        Task<User> FindUserByNameAsync(string name);
        Task<User> FindUserByEmailAsync(string email);
        Task<string> GenerateUserEmailConfirmationTokenAsyn(User user)
        Task<bool> ExistsWithEmailAsync(string email);
        Task<bool> ExistsWithUsernameAsync(string username);
        Task<bool> ConfirmUserEmailAsync(User user, string token);
        Task<bool> SignInUserViaPasswordNoLockoutAsync(User user, string password);
        Task<bool> SignInUserViaPasswordWithLockoutAsync(User user, string password);
        Task<bool> UpdateUserAsync(User user);
    }
}
