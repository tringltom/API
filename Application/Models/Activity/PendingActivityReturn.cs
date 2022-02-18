using System;

namespace Application.Models.Activity
{
    public class PendingActivityReturn : ActivityBase
    {
        public DateTimeOffset DateCreated { get; set; }
    }
}
