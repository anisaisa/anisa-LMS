using anisa_lms.Models;
using System.ComponentModel.DataAnnotations;

namespace anisa_lms.DTOs
{
    public class EnrollmentBaseDto
    {
        public StudentStatus Status { get; set; } = StudentStatus.Active;
    }

    public class CreateEnrollmentDto : EnrollmentBaseDto
    {
        [Required]
        public int CourseId { get; set; }
        [Required]
        public string StudentId { get; set; } = "";
    }

    public class UpdateEnrollmentDto : EnrollmentBaseDto { }

    public class EnrollmentDto : EnrollmentBaseDto
    {
        public int Id { get; set; }
        public string StudentFullName { get; set; } = "";
    }
}
