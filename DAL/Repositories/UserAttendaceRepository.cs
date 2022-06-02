using DAL.RepositoryInterfaces;
using Domain;
using Persistence;

namespace DAL.Repositories
{
    internal class UserAttendaceRepository : BaseRepository<UserAttendance>, IUserAttendaceRepository
    {
        public UserAttendaceRepository(DataContext dbContext) : base(dbContext)
        {
        }
    }
}
