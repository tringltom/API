using System;

namespace Application.Models.Activity
{
    public class ApprovedActivityReturn : ActivityBase
    {
        public DateTimeOffset DateApproved { get; set; }
    }
}
