using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models.User;

namespace Application.ServiceInterfaces
{
    public interface IUsersService
    {
        Task<UserArenaEnvelope> GetTopXpUsers(int? limit, int? offset);
    }
}
