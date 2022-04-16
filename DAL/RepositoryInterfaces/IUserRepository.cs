using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<bool> ExistsWithEmailAsyncAsync(string email);
        Task<bool> ExistsWithUsernameAsync(string username);
        Task<IEnumerable<User>> GetRankedUsersAsync(int? limit, int? offset);
        Task<IEnumerable<User>> GetUsersForImageApprovalAsync(int? limit, int? offset);
        Task<int> CountUsersForImageApprovalAsync();
    }
}
