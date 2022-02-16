namespace Domain
{
    public class ActivityMedia
    {
        public int Id { get; set; }
        public virtual Activity Activity { get; set; }
        public string PublicId { get; set; }
        public string Url { get; set; }

    }
}
