using System.Collections.Generic;
using System.Text.Json.Serialization;
using Application.Models.Activity;

namespace Application.Models.User
{
    public class UserBaseResponse
    {
        public int? Id { get; set; }
        public string Token { get; set; }
        public string UserName { get; set; }
        public int CurrentXp { get; set; }
        public int CurrentLevel { get; set; }
        public bool IsDiceRollAllowed { get; set; }
        public List<ActivityCount> ActivityCounts { get; set; }
        public string About { get; set; }
        public Photo Image { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}
