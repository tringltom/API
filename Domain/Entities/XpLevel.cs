using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class XpLevel
    {
        public int Id { get; set; }
        public int Xp { get; set; }
        public virtual List<User> Activities { get; set; }
    }
}
