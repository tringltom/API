using System.Threading.Tasks;
using DAL.RepositoryInterfaces;

namespace DAL
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        Task<bool> CompleteAsync();
    }
}
