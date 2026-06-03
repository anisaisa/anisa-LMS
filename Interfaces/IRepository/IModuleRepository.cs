using anisa_lms.Models;

namespace anisa_lms.Interfaces.IRepository
{
    public interface IModuleRepository
    {
        public Task<List<Module>> GetModulesByCourseAsync(int cId);
        public Task<Module?> GetByIdAsync(int mId);
        public Task CreateAsync(Module module);
        public void DeleteAsync(Module module);
        public Task SaveChangesAsync();
    }
}
