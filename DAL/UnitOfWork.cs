using System.Threading.Tasks;
using DAL.Repositories;
using DAL.RepositoryInterfaces;
using Persistence;

namespace DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _dbContext;

        public UnitOfWork(DataContext dbContext)
        {
            _dbContext = dbContext;
            Users = new UserRepository(dbContext);
            RefreshTokens = new RefreshTokenRepository(dbContext);
            ActivityReviewXps = new ActivityReviewXpRepository(dbContext);
            Activities = new ActivityRepository(dbContext);
            ActivityCreationCounters = new ActivityCreationCounterRepository(dbContext);
            PendingActivities = new PendingActivityRepository(dbContext);
            UserFavorites = new UserFavortitesRepository(dbContext);
            UserReviews = new UserReviewRepository(dbContext);
        }

        public IUserRepository Users { get; private set; }
        public IRefreshTokenRepository RefreshTokens { get; private set; }
        public IActivityReviewXpRepository ActivityReviewXps { get; private set; }
        public IActivityRepository Activities { get; private set; }
        public IActivityCreationCounterRepository ActivityCreationCounters { get; private set; }
        public IPendingActivityRepository PendingActivities { get; private set; }
        public IUserFavoritesRepository UserFavorites { get; private set; }
        public IUserReviewRepository UserReviews { get; private set; }
        public async Task<bool> CompleteAsync() => await _dbContext.SaveChangesAsync() > 0;
        public void Dispose() => _dbContext.Dispose();
    }
}
