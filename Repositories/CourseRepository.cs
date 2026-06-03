using anisa_lms.Data;
using anisa_lms.Interfaces.IRepository;
using anisa_lms.Models;
using Microsoft.EntityFrameworkCore;

namespace anisa_lms.Repositories
{
    public class CourseRepository(AppDbContext context) : ICourseRepository
    {
        private readonly AppDbContext _context = context;

        public async Task CreateAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
        }

        public void Delete(Course course)
        {
            _context.Courses.Remove(course);
        }

        public IQueryable<Course> GetAllQueryable()
        {
            return _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .Include(c => c.Modules)
                .Include(c => c.Assessments)
                .AsNoTracking()
                .OrderByDescending(c => c.CreatedAt);
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<int> GetEnrollmentsCountAsync(int id)
        {
            return await _context.Courses
                .AsNoTracking()
                .Where(c => c.Id == id).SelectMany(c => c.Enrollments).CountAsync();
        }

        public async Task<int> GetModulesCountAsync(int courseId)
        {
            return await _context.Modules
                .CountAsync(m => m.CourseId == courseId);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
