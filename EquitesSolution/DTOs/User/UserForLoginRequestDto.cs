using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.User
{
    public class UserForLoginRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
