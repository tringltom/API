using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.User
{
    public class UserEmailForVerificationRequestDto
    {
        [Required(ErrorMessage = "Token ne sme biti prazan")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Email adresa ne sme biti prazna")]
        public string Email { get; set; }
    }
}
