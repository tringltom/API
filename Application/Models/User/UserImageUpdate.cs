using Microsoft.AspNetCore.Http;

namespace Application.Models.User
{
    public class UserImageUpdate
    {
        public IFormFile[] Images { get; set; }
    }
}
