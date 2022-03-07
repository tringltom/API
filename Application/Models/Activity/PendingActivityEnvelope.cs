using System.Collections.Generic;

namespace Application.Models.Activity
{
    public class PendingActivityEnvelope
    {
        public List<PendingActivityReturn> Activities { get; set; }
        public int ActivityCount { get; set; }
    }
}
