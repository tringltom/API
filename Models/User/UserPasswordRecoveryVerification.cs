
namespace Models.User
{
    public class UserPasswordRecoveryVerification
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
