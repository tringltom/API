using System.Collections.Generic;

namespace Domain.Entities
{
    public class ActivityType
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public virtual List<PendingActivity> PendingActivities { get; set; }
        public virtual List<Activity> Activities { get; set; }

    }
}
