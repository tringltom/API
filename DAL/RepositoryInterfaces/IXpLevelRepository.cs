using System.Threading.Tasks;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface IXpLevelRepository : IBaseRepository<XpLevel>
    {
        Task<int> GetPotentialLevelAsync(int xpValue);
    }
}
