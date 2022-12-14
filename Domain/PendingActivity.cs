using System;
using System.Collections.Generic;

namespace Domain
{
    public class PendingActivity
    {
        public int Id { get; set; }
        public virtual User User { get; set; }
        public ActivityTypeId ActivityTypeId { get; set; }
        public virtual ActivityType ActivityType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Answer { get; set; }
        public string Location { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? XpReward { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public virtual ICollection<PendingActivityMedia> PendingActivityMedias { get; set; }

    }
}
