using System.Threading.Tasks;
using Models.Activity;

namespace Application.ServiceInterfaces
{
    public interface IActivityService
    {
        Task CreateActivityAsync(ActivityCreate user);
    }
}
