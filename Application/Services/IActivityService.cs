using System.Threading.Tasks;
using Models.Activity;

namespace Application.Services
{
    public interface IActivityService
    {
        Task CreateActivityAsync(ActivityCreate user);
    }
}
