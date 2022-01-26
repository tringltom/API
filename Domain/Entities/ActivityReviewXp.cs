namespace Domain.Entities
{
    public class ActivityReviewXp
    {
        public int Id { get; set; }
        public ActivityTypeId ActivityTypeId { get; set; }
        public ReviewTypeId ReviewTypeId { get; set; }
        public int Xp { get; set; }

        public virtual ActivityType ActivityType { get; set; }
        public virtual ReviewType ReviewType { get; set; }
    }
}
