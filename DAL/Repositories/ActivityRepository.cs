﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Query;
using DAL.RepositoryInterfaces;
using Domain;
using Persistence;

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

        public async Task<int> CountOtherUsersActivitiesAsync(int userId, ActivityQuery activityQuery)
        {
            return await CountAsync(a =>
                a.User.Id != userId
                && (string.IsNullOrEmpty(activityQuery.Title) || a.Title.Contains(activityQuery.Title))
                && (activityQuery.ActivityTypes == null || activityQuery.ActivityTypes.Contains(a.ActivityTypeId)));
        }
    }
}
