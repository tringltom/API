using System;

namespace Domain
{
    public class ActivityCreationCounter
    {
        public int Id { get; set; }
        public virtual User User { get; set; }
        public ActivityTypeId ActivityTypeId { get; set; }
        public virtual ActivityType ActivityType { get; set; }
        public DateTimeOffset DateCreated { get; set; }
    }
}
