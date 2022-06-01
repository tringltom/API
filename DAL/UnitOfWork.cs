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
            Skills = new SkillRepository(dbContext);
            XpLevels = new XpLevelRepository(dbContext);
            SkillSpecials = new SkillSpecialRepostiory(dbContext);
            SkillXpBonuses = new SkillXpBonusRepository(dbContext);
            SkillActivities = new SkillActivityRepository(dbContext);
            UserPuzzleAnswers = new UserPuzzleAnswerRepository(dbContext);
            UserAttendaces = new UserAttendaceRepository(dbContext);
            HappeningMedias = new HappeningMediaRepository(dbContext);
        }

        public IUserRepository Users { get; private set; }
        public IRefreshTokenRepository RefreshTokens { get; private set; }
        public IActivityReviewXpRepository ActivityReviewXps { get; private set; }
        public IActivityRepository Activities { get; private set; }
        public IActivityCreationCounterRepository ActivityCreationCounters { get; private set; }
        public IPendingActivityRepository PendingActivities { get; private set; }
        public IUserFavoritesRepository UserFavorites { get; private set; }
        public IUserReviewRepository UserReviews { get; private set; }
        public ISkillRepository Skills { get; private set; }
        public IXpLevelRepository XpLevels { get; private set; }
        public ISkillSpecialRepository SkillSpecials { get; private set; }
        public ISkillXpBonusRepository SkillXpBonuses { get; private set; }
        public ISkillActivityRepository SkillActivities { get; private set; }
        public IUserPuzzleAnswerRepostiory UserPuzzleAnswers { get; set; }
        public IUserAttendaceRepository UserAttendaces { get; set; }
        public IHappeningMediaRepository HappeningMedias { get; set; }
        public IUserChallengeAnswerRepostiory UserChallengeAnswers { get; set; }
        public async Task<bool> CompleteAsync() => await _dbContext.SaveChangesAsync() > 0;
        public void Dispose() => _dbContext.Dispose();
    }
}
