﻿using Domain;

namespace Models.Activity
{
    public class ActivityCount
    {
        public ActivityTypeId Type { get; set; }
        public int Max { get; set; }
        public int Available { get; set; }
    }
}
