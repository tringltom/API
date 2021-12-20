using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User : IdentityUser<int>
    {
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
        public virtual ICollection<PendingActivity> PendingActivities { get; set; }
        public int CurrentXp { get; set; }
        public int CurrentLevelId { get; set; }
        public virtual XpLevel XpLevel { get; set; }

    }
}
