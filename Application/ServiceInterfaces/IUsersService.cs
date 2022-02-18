using System.Threading.Tasks;
using Application.Models.User;

namespace Application.ServiceInterfaces
{
    public interface IUsersService
    {
        Task<UserRangingEnvelope> GetRangingUsers(int? limit, int? offset);
    }
}
