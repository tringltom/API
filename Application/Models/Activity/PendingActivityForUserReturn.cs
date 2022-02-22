using System;
using Domain;

namespace Application.Models.Activity
{
    public class PendingActivityForUserReturn
    {
        public int Id { get; set; }
        public ActivityTypeId Type { get; set; }
        public string Title { get; set; }
        public DateTimeOffset DateCreated { get; set; }
    }
}
