using System.Threading.Tasks;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface IUserPuzzleAnswerRepostiory : IBaseRepository<UserPuzzleAnswer>
    {
        Task<UserPuzzleAnswer> GetUserPuzzleAnswerAsync(int userId, int activityId);
    }
}
