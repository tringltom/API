namespace Domain.Entities
{
    public class PendingActivityMedia
    {
        public int ID { get; set; }
        public virtual PendingActivity ActivityPending { get; set; }
        public string PublicID { get; set; }
        public string Url { get; set; }

    }
}
