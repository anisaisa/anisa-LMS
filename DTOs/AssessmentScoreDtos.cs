namespace anisa_lms.DTOs
{
    public class AssessmentScoreBaseDto
    {
        public decimal Score { get; set; }
    }

    public class CreateAssessmentScoreDto : AssessmentScoreBaseDto
    {
        public string StudentId { get; set; } = "";
        public int AssessmentId { get; set; }
    }

    public class UpdateAssessmentScoreDto : AssessmentScoreBaseDto { }

    public class AssessmentScoreDto : AssessmentScoreBaseDto
    {
        public int Id { get; set; }
        public string StudentFullName { get; set; } = "";
    }
}
