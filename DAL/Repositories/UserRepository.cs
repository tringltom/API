using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.RepositoryInterfaces;
using Domain;
using Persistence;

namespace DAL.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(DataContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsWithEmailAsync(string email) => await AnyAsync(u => u.Email == email);
        public async Task<bool> ExistsWithUsernameAsync(string username) => await AnyAsync(u => u.UserName == username);
        public async Task<IEnumerable<User>> GetRangingUsers(int? limit, int? offset) => await FindAsync(limit, offset, u => true, u => u.CurrentXp);
        public async Task<IEnumerable<User>> GetUsersForImageApproval(int? limit, int? offset)
        {
            return await FindAsync(limit,
                offset,
                u => !u.ImageApproved && !string.IsNullOrEmpty(u.ImagePublicId),
                u => u.Id);
        }
        public async Task<int> CountUsersForImageApproval() => await CountAsync(u => !u.ImageApproved && !string.IsNullOrEmpty(u.ImagePublicId));
    }
}

