using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.User
{
    public class UserForEmailVerificationRequestDto
    {
        public string Token { get; set; }
        public string Email { get; set; }
    }
}
