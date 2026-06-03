using anisa_lms.Models;

namespace anisa_lms.Interfaces.IRepository
{
    public interface IAssessmentScoreRepository
    {
        public Task<AssessmentScore?> GetByIdAsync(int asId);
        public Task CreateAsync(AssessmentScore assessmentScore);
        public void DeleteAsync(AssessmentScore assessmentScore);
        public Task SaveChangesAsync();
    }
}
