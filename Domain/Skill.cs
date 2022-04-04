namespace Domain
{
    public class Skill
    {
        public int Id { get; set; }
        public virtual User User { get; set; }
        public ActivityTypeId ActivityTypeId { get; set; }
        public virtual ActivityType ActivityType { get; set; }
        public int Level { get; set; }

        public bool IsInFirstTree()
        {
            return Level >= 1 && Level <= 3;
        }
        public bool IsInSecondTree()
        {
            return Level >= 4 && Level <= 6;
        }

        public bool IsInThirdTree()
        {
            return Level == 7;
        }
    }
}
