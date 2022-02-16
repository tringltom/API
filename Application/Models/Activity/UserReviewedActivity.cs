using Domain;

namespace Models.Activity
{
    public class UserReviewedActivity
    {
        public int ActivityId { get; set; }
        public ReviewTypeId ReviewTypeId { get; set; }
    }
}
