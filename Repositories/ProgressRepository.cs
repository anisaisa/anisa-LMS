using anisa_lms.Data;
using anisa_lms.Interfaces.IRepository;
using anisa_lms.Models;
using Microsoft.EntityFrameworkCore;

namespace anisa_lms.Repositories
{
    public class ProgressRepository(AppDbContext context) : IProgressRepository
    {
        private readonly AppDbContext _context = context;

        public async Task CreateAsync(StudentModuleProgress moduleProgress)
        {
            await _context.ModuleProgresses.AddAsync(moduleProgress);
        }

        public void DeleteAsync(StudentModuleProgress moduleProgress)
        {
            _context.ModuleProgresses.Remove(moduleProgress);
        }

        public async Task<StudentModuleProgress?> GetByIdAsync(int pId)
        {
            return await _context.ModuleProgresses
                .Include(p => p.Module)
                .FirstOrDefaultAsync(p => p.Id == pId);
        }

        public async Task<StudentModuleProgress?> GetByStudentAndModuleAsync(string studentId, int moduleId)
        {
            return await _context.ModuleProgresses.AsNoTracking()
                .Include(p => p.Module)
                .FirstOrDefaultAsync(p => p.StudentId == studentId && p.ModuleId == moduleId);
        }

        public async Task<List<StudentModuleProgress>> GetProgressByStudentAsync(string studentId, int cId)
        {
            return await _context.ModuleProgresses.AsNoTracking()
                .Include(mp => mp.Module)
                .Where(mp => mp.StudentId == studentId && mp.Module!.CourseId == cId)
                .ToListAsync();
        }

        public async Task<int> GetCompletedModulesCountAsync(
    string studentId,
    int courseId)
        {
            return await _context.ModuleProgresses
                .CountAsync(mp =>
                    mp.StudentId == studentId &&
                    mp.IsCompleted &&
                    mp.Module.CourseId == courseId);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
