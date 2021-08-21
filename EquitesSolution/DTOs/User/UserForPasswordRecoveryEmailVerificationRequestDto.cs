namespace API.DTOs.User
{
    public class UserForPasswordRecoveryEmailVerificationRequestDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
