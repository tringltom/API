using Domain;

namespace Application.InfrastructureInterfaces.Security
{
    public interface ITokenManager
    {
        string CreateJWTToken(User user);
        RefreshToken CreateRefreshToken();
    }
}
