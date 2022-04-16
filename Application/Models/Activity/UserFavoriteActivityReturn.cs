namespace Application.Models.Activity
{
    public class UserFavoriteActivityReturn
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ActivityId { get; set; }
        public string UserName { get; set; }
        public string ActivityName { get; set; }
    }
}
