using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Security
{
    public interface IJwtGenerator
    {
        string CreateToken(User user);
        RefreshToken GetRefreshToken();
    }
}
