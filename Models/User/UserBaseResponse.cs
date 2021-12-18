
using System.Text.Json.Serialization;

namespace Models.User
{
    public class UserBaseResponse
    {
        public UserBaseResponse(string token, string userName, string refreshToken)
        {
            Token = token;
            Username = userName;
            RefreshToken = refreshToken;
        }
        public UserBaseResponse(string token, string userName, string refreshToken, int userCurrentLevel, int userCurrentXp)
        {
            Token = token;
            Username = userName;
            RefreshToken = refreshToken;
            CurrentLevel = userCurrentLevel.ToString();
            CurrentXp = userCurrentXp.ToString();
        }

        public string Token { get; set; }
        public string Username { get; set; }
        public string CurrentXp { get; set; }
        public string CurrentLevel { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}
