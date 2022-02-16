using System.Collections.Generic;

namespace Domain
{
    public class XpLevel
    {
        public int Id { get; set; }
        public int Xp { get; set; }
        public virtual List<User> Users { get; set; }
    }
}
