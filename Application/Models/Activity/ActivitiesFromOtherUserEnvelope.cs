using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models.Activity
{
    public class ActivitiesFromOtherUserEnvelope
    {
        public List<OtherUserActivityReturn> Activities { get; set; }
        public int ActivityCount { get; set; }
    }
}
