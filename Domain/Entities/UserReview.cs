namespace Domain.Entities
{
    public class UserReview
    {
        public int Id { get; set; }
        public virtual User User { get; set; }
        public virtual Activity Activity { get; set; }
        public virtual ReviewType ReviewType { get; set; }
    }
}
