using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Models.Activity
{
    public class OtherUserActivityReturn : ActivityBase
    {
        public int NumberOfAttendees { get; set; }
        public bool IsUserAttending { get; set; }
        public bool IsHeld { get; set; }
        public bool IsHost { get; set; }
    }
}
