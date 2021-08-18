namespace Application.Models
{
    public class CurrentUserServiceResponse
    {
        public CurrentUserServiceResponse(string username, string token)
        {
            Token = token;
            Username = username;
        }

        public string Token { get; set; }
        public string Username { get; set; }
    }
}
