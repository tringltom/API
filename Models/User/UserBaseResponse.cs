
using System;
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

        public UserBaseResponse(string token, string userName, string refreshToken, int userCurrentLevel, int userCurrentXp, DateTimeOffset? lastRollDate)
        {
            Token = token;
            Username = userName;
            RefreshToken = refreshToken;
            CurrentLevel = userCurrentLevel;
            CurrentXp = userCurrentXp;
            IsDiceRollAllowed = lastRollDate == null || (DateTimeOffset.Now - lastRollDate) >= TimeSpan.FromDays(1);
        }

        public UserBaseResponse(string token, string userName, string refreshToken, int userCurrentLevel, int userCurrentXp, DateTimeOffset? lastRollDate, int id)
        {
            Token = token;
            Username = userName;
            RefreshToken = refreshToken;
            CurrentLevel = userCurrentLevel;
            CurrentXp = userCurrentXp;
            IsDiceRollAllowed = lastRollDate == null || (DateTimeOffset.Now - lastRollDate) >= TimeSpan.FromDays(1);
            Id = id;
        }

        public int? Id { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
        public int CurrentXp { get; set; }
        public int CurrentLevel { get; set; }
        public bool IsDiceRollAllowed { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}
