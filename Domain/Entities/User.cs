using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User : IdentityUser<int>
    {
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
        public virtual ICollection<PendingActivity> PendingActivities { get; set; }

    }
}
