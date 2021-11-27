
namespace Models.User
{
    public class UserLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool StayLoggedIn { get; set; }
    }
}
