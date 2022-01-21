using System.Collections.Generic;

namespace Models.Activity
{
    public class ActivityEnvelope
    {
        public List<ActivityGet> Activities { get; set; }
        public int ActivityCount { get; set; }
    }
}
