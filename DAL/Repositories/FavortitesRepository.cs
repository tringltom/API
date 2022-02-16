using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.RepositoryInterfaces;
using Domain;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Z.EntityFramework.Plus;

namespace DAL.Repositories
{
    public class FavortitesRepository : IFavoritesRepository
    {
        private readonly DataContext _context;

        public FavortitesRepository(DataContext context)
        {
            _context = context;
        }

        private IQueryable<UserFavoriteActivity> GetAllFavoriteActiviesAsQeuriable()
        {
            return _context.UserFavoriteActivities.AsQueryable();
        }

        private DbSet<UserFavoriteActivity> GetAllFavoriteActivies()
        {
            return _context.UserFavoriteActivities;
        }

        public async Task AddFavoriteActivityAsync(UserFavoriteActivity userFavoriteActivity)
        {
            GetAllFavoriteActivies().Add(userFavoriteActivity);
            await _context.SaveChangesAsync();
        }

        public async Task<UserFavoriteActivity> GetFavoriteActivityAsync(UserFavoriteActivity activity)
        {
            return await GetAllFavoriteActiviesAsQeuriable().FirstOrDefaultAsync(fa => fa.UserId == activity.UserId && fa.ActivityId == activity.ActivityId);
        }

        public async Task<IList<UserFavoriteActivity>> GetFavoriteActivitiesByUserIdAsync(int id)
        {
            return await GetAllFavoriteActiviesAsQeuriable().Where(fa => fa.User.Id == id).ToListAsync();
        }

        public async Task<UserFavoriteActivity> GetFavoriteActivityByIdAsync(int id)
        {
            return await GetAllFavoriteActiviesAsQeuriable().FirstOrDefaultAsync(fa => fa.Id == id);
        }

        public async Task<bool> RemoveFavoriteActivityByActivityAndUserIdAsync(int userId, int activityId)
        {
            return await GetAllFavoriteActivies().Where(fa => fa.UserId == userId && fa.ActivityId == activityId).DeleteAsync() > 0;
        }
    }
}
