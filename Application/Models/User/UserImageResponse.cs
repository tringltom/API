namespace Application.Models.User
{
    public class UserImageResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public Photo Image { get; set; }
    }
}
