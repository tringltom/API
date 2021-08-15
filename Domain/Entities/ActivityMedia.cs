namespace Domain.Entities
{
    public class ActivityMedia
    {
        public int ID { get; set; }
        public virtual Activity Activity { get; set; }
        public string PublicID { get; set; }
        public string Url { get; set; }

    }
}
