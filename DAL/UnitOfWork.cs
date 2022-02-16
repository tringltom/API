using System.Threading.Tasks;
using DAL.Repositories;
using DAL.RepositoryInterfaces;
using Persistence;

namespace DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _dbContext;

        public UnitOfWork(DataContext dbContext)
        {
            _dbContext = dbContext;
            Users = new UserRepository(_dbContext);
        }

        public IUserRepository Users { get; private set; }
        public IRefreshTokenRepository RefreshTokens { get; private set; }
        public async Task<bool> CompleteAsync() => await _dbContext.SaveChangesAsync() > 0;
        public void Dispose() => _dbContext.Dispose();
    }
}
