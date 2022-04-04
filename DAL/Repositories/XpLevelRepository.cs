using System.Linq;
using System.Threading.Tasks;
using DAL.RepositoryInterfaces;
using Domain;
using Persistence;

namespace DAL.Repositories
{
    public class XpLevelRepository : BaseRepository<XpLevel>, IXpLevelRepository
    {
        public XpLevelRepository(DataContext dbContext) : base(dbContext)
        {
        }

        public async Task<int> GetPotentialLevel(int xpValue)
        {
            var xpLevels = await GetAllAsync();
            return xpLevels.FirstOrDefault(xp => xp.Xp > xpValue)?.Id - 1 ?? xpLevels.Last().Id;
        }
    }
}
