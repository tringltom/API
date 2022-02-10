using Domain.Entities;

namespace Models.Activity
{
    public class ActivityReview
    {
        public int ActivityId { get; set; }
        public ReviewTypeId ReviewTypeId { get; set; }
    }
}
