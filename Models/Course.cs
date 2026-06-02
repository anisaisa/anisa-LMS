using System.ComponentModel.DataAnnotations;

namespace anisa_lms.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string? InstructorId { get; set; } = "";
        [Required]
        public string Title { get; set; } = "";
        public string? Description { get; set; } = "";
        public CourseStatus Status { get; set; } = CourseStatus.Draft;
        [Required]
        public int MaxEnrollments { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = [];
        public ICollection<Module> Modules { get; set; } = [];
        public ICollection<Assessment> Assessments { get; set; } = [];

        public AppUser? Instructor { get; set; }
    }

    public enum CourseStatus
    {
        Draft = 0,
        Published = 1,
        Scheduled = 2,
        Private = 3,
        Archived = 4
    }
}
