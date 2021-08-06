using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User : IdentityUser
    {
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
