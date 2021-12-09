using System.Collections.Generic;

namespace Models.Activity
{
    public class PendingActivityEnvelope
    {
        public List<PendingActivityGet> Activities { get; set; }
        public int ActivityCount { get; set; }
    }
}
