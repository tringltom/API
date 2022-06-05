﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Query;
using Domain;

namespace DAL.RepositoryInterfaces
{
    public interface IActivityRepository : IBaseRepository<Activity>
    {
        Task<IEnumerable<Activity>> GetOrderedActivitiesFromOtherUsersAsync(ActivityQuery activityQuery, int userId);
        Task<IEnumerable<Activity>> GetActivitiesCreatedByUser(int userId, UserQuery userQuery);
        Task<int> CountActivitiesCreatedByUser(int userId);
        Task<IEnumerable<Activity>> GetHappeningsForApprovalAsync(QueryObject queryObject);
        Task<int> CountOtherUsersActivitiesAsync(int userId, ActivityQuery activityQuery);
        Task<int> CountHappeningsForApprovalAsync();
    }
}
