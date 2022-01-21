using Domain.Entities;

namespace Models.Activity
{
    public class ActivityReviewedByUser
    {
        public int ActivityId { get; set; }
        public ReviewTypeId ReviewTypeId { get; set; }
    }
}
