using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Errors;
using Application.Models.Activity;
using LanguageExt;

namespace Application.ServiceInterfaces
{
    public interface IReviewService
    {
        Task<IList<UserReviewedActivity>> GetOwnerReviewsAsync();
        Task<Either<RestError, Unit>> ReviewActivityAsync(ActivityReview activityReview);
    }
}
