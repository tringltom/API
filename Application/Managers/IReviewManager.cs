using System.Threading.Tasks;
using Models.Activity;

namespace Application.Managers
{
    public interface IReviewManager
    {
        Task ReviewActivityAsync(ActivityReview activityReview);
    }
}
