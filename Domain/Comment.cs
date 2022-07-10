using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Body { get; set; }
        public virtual User User { get; set; }
        public virtual Activity Activity { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
