using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Query;
using DAL.RepositoryInterfaces;
using Domain;
using Persistence;

namespace DAL.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(DataContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsWithEmailAsyncAsync(string email) => await AnyAsync(u => u.Email == email);
        public async Task<bool> ExistsWithUsernameAsync(string username) => await AnyAsync(u => u.UserName == username);
        public async Task<IEnumerable<User>> GetRankedUsersAsync(UserQuery userQuery)
        {
            return await FindAsync(userQuery.Limit,
                userQuery.Offset,
                u => string.IsNullOrEmpty(userQuery.UserName) || u.UserName.Contains(userQuery.UserName),
                u => u.CurrentXp);
        }
        public async Task<int> CountRankedUsersAsync(UserQuery userQuery)
        {
            return await CountAsync(u => string.IsNullOrEmpty(userQuery.UserName) || u.UserName.Contains(userQuery.UserName));

        }
        public async Task<IEnumerable<User>> GetUsersForImageApprovalAsync(QueryObject queryObject)
        {
            return await FindAsync(queryObject.Limit,
                queryObject.Offset,
                u => !u.ImageApproved && !string.IsNullOrEmpty(u.ImagePublicId),
                u => u.Id);
        }
        public async Task<int> CountUsersForImageApprovalAsync() => await CountAsync(u => !u.ImageApproved && !string.IsNullOrEmpty(u.ImagePublicId));
    }
}

