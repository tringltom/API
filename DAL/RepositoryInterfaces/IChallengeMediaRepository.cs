using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface IChallengeMediaRepository : IBaseRepository<ChallengeMedia>
    {
        Task<IEnumerable<ChallengeMedia>> GetChallengeMedias(int userChallengeAnswerId);
    }
}
