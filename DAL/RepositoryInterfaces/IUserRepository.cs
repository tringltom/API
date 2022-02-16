using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<bool> ExistsWithEmailAsync(string email);
        Task<bool> ExistsWithUsernameAsync(string username);
        Task<IEnumerable<User>> GetTopXpUsersAsync(int? limit, int? offset);
    }
}
