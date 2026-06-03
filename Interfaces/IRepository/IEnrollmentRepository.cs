using anisa_lms.Models;

namespace anisa_lms.Interfaces.IRepository
{
    public interface IEnrollmentRepository
    {
        public Task<Enrollment?> GetByIdAsync(int id);
        public Task CreateAsync(Enrollment enrollment);
        public void DeleteAsync(Enrollment enrollment);
        public Task SaveChangesAsync();
        public Task<List<Enrollment>> GetAllAsync();

      public Task<Enrollment?> GetByStudentAndCourseAsync(string studentId, int courseId); //anisa
    }
}
