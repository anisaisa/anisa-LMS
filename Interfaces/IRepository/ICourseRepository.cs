using anisa_lms.Models;

namespace anisa_lms.Interfaces.IRepository
{
    public interface ICourseRepository
    {
        public IQueryable<Course> GetAllQueryable();
        public Task<Course?> GetByIdAsync(int id);
        public Task<int> GetEnrollmentsCountAsync(int id);
        public Task CreateAsync(Course course);
        public void Delete(Course course);
        public Task SaveChangesAsync();

        Task<int> GetModulesCountAsync(int courseId);
    }
}
