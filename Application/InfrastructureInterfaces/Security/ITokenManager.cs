using Domain;

namespace Application.InfrastructureInterfaces.Security
{
    public interface ITokenManager
    {
        string CreateToken(User user);
        RefreshToken CreateRefreshToken();
    }
}
