using anisa_lms.Data;
using anisa_lms.Interfaces.IRepository;
using anisa_lms.Models;
using Microsoft.EntityFrameworkCore;

namespace anisa_lms.Repositories
{
    public class AssessmentRepository(AppDbContext context) : IAssessmentRepository
    {
        private readonly AppDbContext _context = context;

        public async Task CreateAsync(Assessment assessment)
        {
            await _context.Assessments.AddAsync(assessment);
        }

        public void DeleteAsync(Assessment assessment)
        {
            _context.Assessments.Remove(assessment);
        }

        public IQueryable<AssessmentScore> GetAssessmentScores(int aId)
        {
            return _context.AssessmentScores
                .AsNoTracking()
                .Where(s => s.AssessmentId == aId).AsQueryable();
        }

        public async Task<Assessment?> GetByIdAsync(int id)
        {
            return await _context.Assessments.FirstOrDefaultAsync(a => a.Id == id);
        }

        public IQueryable<Assessment> GetUpcomingQueryable(int cId)
        {
            return _context.Assessments
                .AsNoTracking()
                .Where(a => a.CourseId == cId && a.DueDate >= DateTime.UtcNow)
                .AsQueryable();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
