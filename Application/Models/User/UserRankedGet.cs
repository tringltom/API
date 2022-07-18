namespace Application.Models.User
{
    public class UserRankedGet
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public Photo Image { get; set; }
        public string Email { get; set; }
        public int CurrentXp { get; set; }
        public int CurrentLevel { get; set; }
        public int NumberOfGoodDeeds { get; set; }
        public int NumberOfJokes { get; set; }
        public int NumberOfQuotes { get; set; }
        public int NumberOfPuzzles { get; set; }
        public int NumberOfHappenings { get; set; }
        public int NumberOfChallenges { get; set; }
    }
}
