using DAL.Query;
using DAL.RepositoryInterfaces;
using Domain;
using Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ActivityRepository : BaseRepository<Activity>, IActivityRepository
    {
        public ActivityRepository(DataContext context) : base(context) { }

        public async Task<IEnumerable<Activity>> GetOrderedActivitiesFromOtherUsersAsync(ActivityQuery activityQuery, int userId)
        {
            return await FindAsync(activityQuery.Limit,
                activityQuery.Offset,
                a => a.User.Id != userId
                    && (string.IsNullOrEmpty(activityQuery.Title) || a.Title.Contains(activityQuery.Title))
                    && (activityQuery.ActivityTypes == null || activityQuery.ActivityTypes.Contains(a.ActivityTypeId)),
                a => a.Id);
        }

        public async Task<IEnumerable<Activity>> GetHappeningsForApprovalAsync(QueryObject queryObject)
        {
            return await FindAsync(queryObject.Limit,
                queryObject.Offset,
                a => a.ActivityTypeId == ActivityTypeId.Happening
                    && a.HappeningMedias.Count > 0,
                a => a.Id);
        }

        public async Task<IEnumerable<Activity>> GetChallengesForApprovalAsync(QueryObject queryObject)
        {
            return await FindAsync(queryObject.Limit,
                queryObject.Offset,
                a => a.ActivityTypeId == ActivityTypeId.Challenge
                    && a.XpReward == null
                    && a.UserChallengeAnswers.Any(uc => uc.Confirmed),
                a => a.Id);
        }

        public async Task<IEnumerable<Activity>> GetActivitiesCreatedByUser(int userId, ActivityQuery activityQuery)
        {
            return await FindAsync(activityQuery.Limit,
                activityQuery.Offset,
                a => a.User.Id == userId && (string.IsNullOrEmpty(activityQuery.Title) || a.Title.Contains(activityQuery.Title))
                    && (activityQuery.ActivityTypes == null || activityQuery.ActivityTypes.Contains(a.ActivityTypeId)),
                a => a.Id);
        }

        public async Task<IEnumerable<Activity>> GetFavoritedActivitiesByUser(int userId, ActivityQuery activityQuery)
        {
            return await FindAsync(activityQuery.Limit,
                activityQuery.Offset,
                a => a.UserFavorites.Any(a => a.UserId == userId) && (string.IsNullOrEmpty(activityQuery.Title) || a.Title.Contains(activityQuery.Title))
                    && (activityQuery.ActivityTypes == null || activityQuery.ActivityTypes.Contains(a.ActivityTypeId)),
                a => a.Id);
        }

        public async Task<int> CountOtherUsersActivitiesAsync(int userId, ActivityQuery activityQuery)
        {
            return await CountAsync(a =>
                a.User.Id != userId
                && (string.IsNullOrEmpty(activityQuery.Title) || a.Title.Contains(activityQuery.Title))
                && (activityQuery.ActivityTypes == null || activityQuery.ActivityTypes.Contains(a.ActivityTypeId)));
        }
        public async Task<int> CountActivitiesCreatedByUser(int userId)
        {
            return await CountAsync(a => a.User.Id == userId);
        }

        public async Task<int> CountFavoritedActivitiesByUser(int userId)
        {
            return await CountAsync(a => a.UserFavorites.Any(a => a.UserId == userId));
        }

        public async Task<int> CountHappeningsForApprovalAsync()
        {
            return await CountAsync(a =>
                a.ActivityTypeId == ActivityTypeId.Happening
                && a.HappeningMedias != null);
        }

        public async Task<int> CountChallengesForApprovalAsync()
        {
            return await CountAsync(a =>
                a.ActivityTypeId == ActivityTypeId.Challenge
                && a.UserChallengeAnswers.Any(uc => uc.Confirmed));
        }
    }
}
