using System;

namespace Application.Models.Activity
{
    public class ApprovedActivityReturn : ActivityBase
    {
        public DateTimeOffset DateApproved { get; set; }
        public int NumberOfAttendees { get; set; }
        public bool IsUserAttending { get; set; }
        public bool IsHeld { get; set; }
        public bool IsHost { get; set; }
        public bool IChallengeAnswered { get; set; }
    }
}
