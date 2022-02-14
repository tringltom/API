using System;
using System.Collections.Generic;
using System.Text;

namespace Models.User
{
    public class UserArenaEnvelope
    {
        public List<UserArenaGet> Users { get; set; }
        public int UserCount { get; set; }
    }
}
