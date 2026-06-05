using anisa_lms.Models;
using System.ComponentModel.DataAnnotations;

namespace anisa_lms.DTOs
{
    public class CourseBaseDto
    {
        public string? InstructorId { get; set; } = "";
        [MinLength(3, ErrorMessage = "Title must be atleast 3 chars long")]
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public CourseStatus Status { get; set; } = CourseStatus.Draft;
        public int MaxEnrollments { get; set; }
    }

    public class CreateCourseDto : CourseBaseDto { }

    public class UpdateCourseDto : CourseBaseDto { }

    public class CourseDto : CourseBaseDto
    {
        public int Id { get; set; }
        public string InstructorFullName { get; set; } = "";
        public List<EnrollmentDto> Enrollments { get; set; } = [];
        public List<ModuleDto> Modules { get; set; } = [];
        public List<AssessmentDto> Assessments { get; set; } = [];
    }

    public class CourseQueryParams
    {
        public string? Title { get; set; } = "";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
