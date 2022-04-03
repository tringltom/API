namespace Application.InfrastructureInterfaces.Security
{
    public interface ITokenManager
    {
        string CreateJWTToken(int id, string userName);
        string CreateRefreshToken();
    }
}
