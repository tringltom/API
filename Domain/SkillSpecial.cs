using System.Collections.Generic;

namespace Domain
{
    public class SkillSpecial
    {
        public int Id { get; set; }
        public ActivityTypeId ActivityTypeOneId { get; set; }
        public virtual ActivityType ActivityTypeOne { get; set; }
        public ActivityTypeId? ActivityTypeTwoId { get; set; }
        public virtual ActivityType ActivityTypeTwo { get; set; }
        public string Title { get; set; }

        public virtual ICollection<User> Users { get; set; }

    }
}
