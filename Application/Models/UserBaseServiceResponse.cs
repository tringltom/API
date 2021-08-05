namespace Application.Models
{
    public class UserBaseServiceResponse
    {
        public UserBaseServiceResponse(string token, string userName, string refreshToken)
        {
            Token = token;
            Username = userName;
            RefreshToken = refreshToken;
        }
        public string Token { get; private set; }
        public string Username { get; private set; }
        public string RefreshToken { get; private set; }
    }
}
