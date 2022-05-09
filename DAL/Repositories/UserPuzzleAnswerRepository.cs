using System.Threading.Tasks;
using DAL.RepositoryInterfaces;
using Domain;
using Persistence;

namespace DAL.Repositories
{
    internal class UserPuzzleAnswerRepository : BaseRepository<UserPuzzleAnswer>, IUserPuzzleAnswerRepostiory
    {
        public UserPuzzleAnswerRepository(DataContext dbContext) : base(dbContext) { }

        public async Task<UserPuzzleAnswer> GetUserPuzzleAnswerAsync(int userId, int activityId)
        {
            return await GetAsync(up => up.UserId == userId && up.ActivityId == activityId);
        }
    }
}
