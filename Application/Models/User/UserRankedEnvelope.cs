using System.Collections.Generic;

namespace Application.Models.User
{
    public class UserRankedEnvelope
    {
        public List<UserRankedGet> Users { get; set; }
        public int UserCount { get; set; }
    }
}
