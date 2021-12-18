namespace Domain.Entities
{
    public class UserFavoriteActivity
    {
        public int Id { get; set; }
        public virtual User User { get; set; }
        public virtual Activity Activity { get; set; }
    }
}
