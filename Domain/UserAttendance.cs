namespace Domain
{
    public class UserAttendance
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int ActivityId { get; set; }
        public virtual Activity Activity { get; set; }

        public bool Confirmed { get; set; }
    }
}
