using System.Threading.Tasks;
using Domain.Entities;

namespace Application.RepositoryInterfaces
{
    public interface IActivityRepository
    {
        Task CreateActivityAsync(PendingActivity activity);

        Task<Activity> GetActivityByIdAsync(int id);
    }
}
