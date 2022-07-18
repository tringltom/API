using Domain;

namespace Application.Models.Activity
{
    public class UserReviewedActivity
    {
        public int ActivityId { get; set; }
        public ReviewTypeId ReviewTypeId { get; set; }
        public int UserId { get; set; }
    }
}
