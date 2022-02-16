using System.Threading.Tasks;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
    {
        Task<RefreshToken> GetOldRefreshToken(string refreshToken);
    }
}
