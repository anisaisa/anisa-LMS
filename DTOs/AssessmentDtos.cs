using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace anisa_lms.DTOs
{
    public class AssessmentBaseDto
    {
        [Required]
        public string Title { get; set; } = "";
        [Precision(18, 2)]
        public decimal MaxPoints { get; set; }
        public decimal PassRequirement { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class CreateAssessmentDto : AssessmentBaseDto
    {
        [Required]
        public int CourseId { get; set; }
    }

    public class UpdateAssessmentDto : AssessmentBaseDto { }

    public class AssessmentDto : AssessmentBaseDto {
        public int Id { get; set; }
    }
}
