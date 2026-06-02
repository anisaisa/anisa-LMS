using Microsoft.EntityFrameworkCore;

namespace anisa_lms.Models
{
    public class AssessmentScore
    {
        public int Id { get; set; }
        public string? StudentId { get; set; } = "";
        public int AssessmentId { get; set; }
        [Precision(18, 2)]
        public decimal Score { get; set; }

        public AppUser? Student { get; set; }
        public Assessment? Assessment { get; set; }
    }
}
