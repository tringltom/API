using System.Collections.Generic;

namespace Application.Models.Activity
{
    public class PendingActivityForUserEnvelope
    {
        public List<PendingActivityForUserReturn> Activities { get; set; }
        public int ActivityCount { get; set; }
    }
}
