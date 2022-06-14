using System.Collections.Generic;

namespace Domain
{
    public class UserChallengeAnswer
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int ActivityId { get; set; }
        public virtual Activity Activity { get; set; }

        public string Description { get; set; }
        public bool Confirmed { get; set; }
        public virtual ICollection<ChallengeMedia> ChallengeMedias { get; set; }
    }
}
