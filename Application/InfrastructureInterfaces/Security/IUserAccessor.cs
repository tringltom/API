namespace Application.InfrastructureInterfaces.Security
{
    public interface IUserAccessor
    {
        string GetUsernameFromAccesssToken();
        int GetUserIdFromAccessToken();
    }
}
