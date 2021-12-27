using System.Threading.Tasks;
using Models.Activity;

namespace Application.ServiceInterfaces
{
    public interface IActivityReviewService
    {
        Task ReviewActivityAsync(ActivityReview activityReview);
    }
}
