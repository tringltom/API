using System.Threading.Tasks;
using Models.User;

namespace Application.ServiceInterfaces
{
    public interface IUsersService
    {
        Task<UserRangingEnvelope> GetTopXpUsers(int? limit, int? offset);
    }
}
