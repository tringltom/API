using System.ComponentModel.DataAnnotations;

namespace API.DTOs.User
{
    public class UserForRegistrationRequestDto
    {
        [Required(ErrorMessage = "Korisnicko ime je neophodno")]
        [StringLength(30, ErrorMessage = "Korisnicko ime ne moze biti duze od 30 karaktera")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email adresa je neophodna")]
        [StringLength(60, ErrorMessage = "Email adresa ne moze biti duza od 60 karaktera")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Sifra je neophodna")]
        [StringLength(60, ErrorMessage = "Sifra ne moze biti duza od 60 karaktera")]
        public string Password { get; set; }

    }
}
