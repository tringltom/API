using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class UserReview
    {
        public int Id { get; set; }


        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int ActivityId { get; set; }
        public virtual Activity Activity { get; set; }

        [Required]
        public virtual ReviewType ReviewType { get; set; }
    }
}
