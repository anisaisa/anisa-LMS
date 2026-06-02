namespace anisa_lms.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        public string StudentId { get; set; } = "";
        public int CourseId { get; set; }
        public StudentStatus Status { get; set; } = StudentStatus.Active;
        public DateTime EnrolledAt { get; set; }

        public AppUser? Student { get; set; }
        public Course? Course { get; set; }
    }

    public enum StudentStatus
    {
        Active = 1,
        Completed = 2,
        Dropped = 3,
        PendingPayment = 4
    }
}
