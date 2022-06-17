namespace Domain
{
    public class ChallengeMedia
    {
        public int Id { get; set; }
        public virtual UserChallengeAnswer UserChallengeAnswer { get; set; }
        public string PublicId { get; set; }
        public string Url { get; set; }
    }
}
