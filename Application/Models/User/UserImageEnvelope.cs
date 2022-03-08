using System.Collections.Generic;

namespace Application.Models.User
{
    public class UserImageEnvelope
    {
        public List<UserImageResponse> Users { get; set; }
        public int UserCount { get; set; }
    }
}
