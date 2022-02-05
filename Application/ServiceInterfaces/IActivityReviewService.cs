﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Models.Activity;

namespace Application.ServiceInterfaces
{
    public interface IActivityReviewService
    {
        Task<UserReview> GetUserReviewByActivityAndUserId(int activityId, int userId);
        Task UpdateReviewActivityAsync(ActivityReview activityReview);
        Task AddReviewActivityAsync(ActivityReview activityReview);
        Task<IList<UserReviewedActivity>> GetAllReviewsByUserId(int userId);
    }
}
