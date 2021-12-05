namespace Domain.Entities;

public class PendingActivityMedia
{
    public int Id { get; set; }
    public virtual PendingActivity ActivityPending { get; set; }
    public string PublicId { get; set; }
    public string Url { get; set; }

}

