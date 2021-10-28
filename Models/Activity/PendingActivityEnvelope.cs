using System.Collections.Generic;
using Domain.Entities;

namespace Models.Activity
{
    public class PendingActivityEnvelope
    {
        public List<PendingActivity> Activities { get; set; }
        public int ActivityCount { get; set; }
    }
}
