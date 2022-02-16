using System.Threading.Tasks;
using DAL.RepositoryInterfaces;
using Domain;
using Persistence;

namespace DAL.Repositories
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(DataContext dbContext) : base(dbContext) { }

        public async Task<RefreshToken> GetOldRefreshToken(string refreshToken) => await GetAsync(rt => rt.Token == refreshToken);
    }
}
