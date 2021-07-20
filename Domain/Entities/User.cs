using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class User : IdentityUser
    {
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
