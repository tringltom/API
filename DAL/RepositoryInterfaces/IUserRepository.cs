using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Query;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<bool> ExistsWithEmailAsyncAsync(string email);
        Task<bool> ExistsWithUsernameAsync(string username);
        Task<IEnumerable<User>> GetRankedUsersAsync(UserQuery userQuery);
        Task<IEnumerable<User>> GetUsersForImageApprovalAsync(QueryObject queryObject);
        Task<int> CountUsersForImageApprovalAsync();
    }
}
