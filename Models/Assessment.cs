using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace anisa_lms.Models
{
    public class Assessment
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        [Required]
        public string Title { get; set; } = "";
        [Precision(18, 2)]
        public decimal MaxPoints { get; set; }
        [Precision(18, 2)]
        public decimal PassRequirement { get; set; }
        public DateTime DueDate { get; set; }
        public ICollection<AssessmentScore> AssessmentScores { get; set; } = [];

        public Course? Course { get; set; }
    }
}
