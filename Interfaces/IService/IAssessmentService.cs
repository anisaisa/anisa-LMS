using anisa_lms.DTOs;

namespace anisa_lms.Interfaces.IService
{
    public interface IAssessmentService
    {
        public Task<List<AssessmentDto>> GetUpcomingAssessments(int cId);
        public Task<List<AssessmentScoreDto>> GetResults(int aId, bool passed);
        public Task CreateAssessment(CreateAssessmentDto create,string instructorId, bool isAdmin);
        public Task<bool?> UpdateAssessment(int aId, UpdateAssessmentDto update,string instructorId,bool isAdmin);
        public Task<bool?> DeleteAssessment(int aId,string instructorId,bool isAdmin);
    }
}
