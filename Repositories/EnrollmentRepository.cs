using anisa_lms.Data;
using anisa_lms.Interfaces.IRepository;
using anisa_lms.Models;
using Microsoft.EntityFrameworkCore;

namespace anisa_lms.Repositories
{
    public class EnrollmentRepository(AppDbContext context) : IEnrollmentRepository
    {
        private readonly AppDbContext _context = context;

        public async Task CreateAsync(Enrollment enrollment)
        {
            await _context.Enrollments.AddAsync(enrollment);
        }

        public void DeleteAsync(Enrollment enrollment)
        {
            _context.Enrollments.Remove(enrollment);
        }

        public async Task<List<Enrollment>> GetAllAsync()
        {
            return await _context.Enrollments
                .AsNoTracking()
                .Include(e => e.Student)
                .Include(e => e.Course)
                .ToListAsync();
        }

        public async Task<Enrollment?> GetByIdAsync(int id)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Enrollment?> GetByStudentAndCourseAsync(
    string studentId,
    int courseId)
        {
            return await _context.Enrollments
                .FirstOrDefaultAsync(e =>
                    e.StudentId == studentId &&
                    e.CourseId == courseId); 
        }
    }
}
