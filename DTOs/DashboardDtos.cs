using anisa_lms.Models;

namespace anisa_lms.DTOs
{
    public class AdminDashboardDto
    {
        public int TotalUsers { get; set; }
        public int TotalCourses { get; set; }
        public List<DashboardCourseDto> PopularCourses { get; set; } = [];
        public List<DashboardCourseDto> RecentCourses { get; set; } = [];
    }

    public class InstructorDashboardDto
    {
        public List<Course> MyCourses { get; set; } = [];
        public List<Course> RecentCourses { get; set; } = [];
        public List<ICollection<Assessment>> Assessments { get; set; } = [];
        public int StudentsEnrolled { get; set; }
    }

    public class StudentDashboardDto
    {
        public List<Course?> CoursesInProgress { get; set; } = [];
        public int CompletedAssessments { get; set; }
        public int TotalEnrollments { get; set; }
        public int ModulesCompleted { get; set; }
    }

    public class DashboardCourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int MaxEnrollments { get; set; }
        public int EnrollmentCount { get; set; }
        public string InstructorFullName { get; set; } = "";
    }
}
