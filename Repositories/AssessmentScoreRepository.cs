using anisa_lms.Data;
using anisa_lms.Interfaces.IRepository;
using anisa_lms.Models;
using Microsoft.EntityFrameworkCore;

namespace anisa_lms.Repositories
{
    public class AssessmentScoreRepository(AppDbContext context) : IAssessmentScoreRepository
    {
        private readonly AppDbContext _context = context;

        public async Task CreateAsync(AssessmentScore assessmentScore)
        {
            await _context.AssessmentScores.AddAsync(assessmentScore);
        }

        public void DeleteAsync(AssessmentScore assessmentScore)
        {
            _context.AssessmentScores.Remove(assessmentScore);
        }

        public async Task<AssessmentScore?> GetByIdAsync(int asId)
        {
            return await _context.AssessmentScores.FirstOrDefaultAsync(a => a.Id == asId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
