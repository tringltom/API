
namespace Models.User
{
    public class UserCurrentlyLoggedIn
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public int CurrentXp { get; set; }
        public int CurrentLevel { get; set; }
    }
}
