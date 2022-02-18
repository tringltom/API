
namespace Application.Models.User
{
    public class UserPasswordChange
    {
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
