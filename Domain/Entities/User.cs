using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User : IdentityUser<int>
    {
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
        public virtual ICollection<PendingActivity> PendingActivities { get; set; }
        public virtual ICollection<ActivityCreationCounter> ActivityCreationCounters { get; set; }
        public int CurrentXp { get; set; }
        public int XpLevelId { get; set; }
        public virtual XpLevel XpLevel { get; set; }
        public DateTimeOffset? LastRollDate { get; set; }
    }
}
