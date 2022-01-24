
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Models.Activity;

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

        public UserBaseResponse(string token, string userName, string refreshToken, int userCurrentLevel, int userCurrentXp, DateTimeOffset? lastRollDate, List<ActivityCount> activityCounts)
        {
            Token = token;
            Username = userName;
            RefreshToken = refreshToken;
            CurrentLevel = userCurrentLevel;
            CurrentXp = userCurrentXp;
            IsDiceRollAllowed = lastRollDate == null || (DateTimeOffset.Now - lastRollDate) >= TimeSpan.FromDays(1);
            ActivityCounts = activityCounts;
        }

        public string Token { get; set; }
        public string Username { get; set; }
        public int CurrentXp { get; set; }
        public int CurrentLevel { get; set; }
        public bool IsDiceRollAllowed { get; set; }
        public List<ActivityCount> ActivityCounts { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}
