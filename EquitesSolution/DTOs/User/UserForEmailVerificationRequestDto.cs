namespace API.DTOs.User
{
    public class UserForEmailVerificationRequestDto
    {
        public string Token { get; set; }
        public string Email { get; set; }
    }
}
