using System.Collections.Generic;

namespace Application.Models.User
{
    public class UserRangingEnvelope
    {
        public List<UserRangingGet> Users { get; set; }
        public int UserCount { get; set; }
    }
}
