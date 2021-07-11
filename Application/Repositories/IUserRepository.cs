
using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IUserRepository
    {
        Task<bool> CreateUserAsync(User user, string password);
        Task<User> FindUserByEmailAsync(string email);
        Task<bool> ExistsWithEmailAsync(string email);
        Task<bool> ExistsWithUsernameAsync(string username);
    }
}
