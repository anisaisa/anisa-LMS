using anisa_lms.Data;
using anisa_lms.Interfaces.IRepository;
using anisa_lms.Models;
using Microsoft.EntityFrameworkCore;

namespace anisa_lms.Repositories
{
    public class ModuleRepository(AppDbContext context) : IModuleRepository
    {
        private readonly AppDbContext _context = context;

        public async Task CreateAsync(Module module)
        {
            await _context.Modules.AddAsync(module);
        }

        public void DeleteAsync(Module module)
        {
            _context.Modules.Remove(module);
        }

        public async Task<Module?> GetByIdAsync(int mId)
        {
            return await _context.Modules.FirstOrDefaultAsync(m => m.Id == mId);
        }

        public async Task<List<Module>> GetModulesByCourseAsync(int cId)
        {
            return await _context.Modules.AsNoTracking().Where(m => m.CourseId == cId).ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
