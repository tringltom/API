using System.Text.Json.Serialization;

namespace Application.Models.User
{
    public class UserRefreshResponse
    {
        public UserRefreshResponse(string accesstoken, string refreshToken)
        {
            Token = accesstoken;
            RefreshToken = refreshToken;
        }

        public string Token { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}
