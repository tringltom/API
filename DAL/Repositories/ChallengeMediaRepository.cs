using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.RepositoryInterfaces;
using Domain;
using Persistence;

namespace DAL.Repositories
{
    public class ChallengeMediaRepository : BaseRepository<ChallengeMedia>, IChallengeMediaRepository
    {
        public ChallengeMediaRepository(DataContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<ChallengeMedia>> GetChallengeMedias(int userChallengeAnswerId)
        {
            return await FindAsync(cm => cm.UserChallengeAnswer.Id == userChallengeAnswerId);
        }
    }
}
