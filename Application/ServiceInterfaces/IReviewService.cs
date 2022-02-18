using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Models.Activity;

namespace Application.ServiceInterfaces
{
    public interface IReviewService
    {
        Task ReviewActivityAsync(ActivityReview activityReview);
        Task<IList<UserReviewedActivity>> GetAllReviews(int userId);
    }
}
