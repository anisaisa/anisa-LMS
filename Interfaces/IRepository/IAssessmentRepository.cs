using anisa_lms.Models;

namespace anisa_lms.Interfaces.IRepository
{
    public interface IAssessmentRepository
    {
        public Task<Assessment?> GetByIdAsync(int id);
        public IQueryable<Assessment> GetUpcomingQueryable(int cId);
        public IQueryable<AssessmentScore> GetAssessmentScores(int aId);
        public Task CreateAsync(Assessment assessment);
        public void DeleteAsync(Assessment assessment);
        public Task SaveChangesAsync();
    }
}
