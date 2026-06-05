using anisa_lms.DTOs;

namespace anisa_lms.Interfaces.IService
{
    public interface IAssessmentScoreService
    {
        public Task CreateAssessmentScore(CreateAssessmentScoreDto create);
        public Task<bool?> UpdateAssessmentScore(int asId, UpdateAssessmentScoreDto update);
        public Task<bool?> DeleteAssessmentScore(int asId);
    }
}
