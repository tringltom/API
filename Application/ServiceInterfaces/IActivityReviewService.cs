using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Models.Activity;

namespace Application.ServiceInterfaces
{
    public interface IActivityReviewService
    {
        Task<UserReview> GetUserReviewByActivityAndUserId(int activityId, int userId);
        Task UpdateReviewActivityAsync(ActivityReview activityReview, int reviwerId);
        Task AddReviewActivityAsync(ActivityReview activityReview, int reviewrId);
        Task<IList<UserReviewedActivity>> GetAllReviewsByUserId(int userId);
    }
}
