using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.User
{
    public class UserForResendEmailVerificationRequestDto
    {
        public string Email { get; set; }
    }
}
