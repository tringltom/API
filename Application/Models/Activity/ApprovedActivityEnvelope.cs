﻿using System.Collections.Generic;

namespace Models.Activity
{
    public class ApprovedActivityEnvelope
    {
        public List<ApprovedActivityReturn> Activities { get; set; }
        public int ActivityCount { get; set; }
    }
}
