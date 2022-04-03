using System.Threading.Tasks;
using DAL.RepositoryInterfaces;

namespace DAL
{
    public interface IUnitOfWork
    {
        IActivityReviewXpRepository ActivityReviewXps { get; }
        IActivityRepository Activities { get; }
        IActivityCreationCounterRepository ActivityCreationCounters { get; }
        IPendingActivityRepository PendingActivities { get; }
        IUserFavoritesRepository UserFavorites { get; }
        IUserReviewRepository UserReviews { get; }
        IUserRepository Users { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        ISkillRepository Skills { get; }
        IXpLevelRepository XpLevels { get; }
        ISkillSpecialRepository SkillSpecials { get; }
        ISkillXpBonusRepository SkillXpBonuses { get; }
        ISkillActivityRepository SkillActivities { get; }
        Task<bool> CompleteAsync();
    }
}
