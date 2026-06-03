using anisa_lms.Models;

namespace anisa_lms.Interfaces.IRepository
{
    public interface IProgressRepository
    {
        public Task<List<StudentModuleProgress>> GetProgressByStudentAsync(string studentId, int cId);
        public Task<StudentModuleProgress?> GetByIdAsync(int pId);
        public Task CreateAsync(StudentModuleProgress moduleProgress);
        public void DeleteAsync(StudentModuleProgress moduleProgress);
        public Task SaveChangesAsync();

        Task<int> GetCompletedModulesCountAsync(
    string studentId,
    int courseId);
    }
}
