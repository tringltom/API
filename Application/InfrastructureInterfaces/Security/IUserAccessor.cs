using System.Threading.Tasks;
using Domain;

namespace Application.InfrastructureInterfaces.Security
{
    public interface IUserAccessor
    {
        Task<User> FindUserFromAccessToken();
        string GetUsernameFromAccesssToken();
        int GetUserIdFromAccessToken();
    }
}
