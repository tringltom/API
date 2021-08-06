using Domain.Entities;

namespace Application.Security
{
    public interface IJwtGenerator
    {
        string CreateToken(User user);
        RefreshToken GetRefreshToken();
    }
}
