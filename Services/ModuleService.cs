using anisa_lms.Data;
using anisa_lms.DTOs;
using anisa_lms.Interfaces.IRepository;
using anisa_lms.Interfaces.IService;
using anisa_lms.Models;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;

namespace anisa_lms.Services
{
    public class ModuleService(
        IModuleRepository repo,
        IProgressRepository progressRepo,
        IMapper mapper,
        IMemoryCache cache) : IModuleService
    {
        private readonly IModuleRepository _repo = repo;
        private readonly IProgressRepository _progressRepo = progressRepo;
        private readonly IMapper _mapper = mapper;
        private readonly IMemoryCache _cache = cache;

        

        public async Task CreateModule(CreateModuleDto create)
        {
            var module = _mapper.Map<Module>(create);

            await _repo.CreateAsync(module);
            await _repo.SaveChangesAsync();

           
        }

        public async Task<bool?> DeleteModule(int mId)
        {
            var module = await _repo.GetByIdAsync(mId);

            if (module == null)
                return null;

            _repo.DeleteAsync(module);
            await _repo.SaveChangesAsync();

            return true;
        }

        public async Task<List<ModuleDto>> GetModulesForStudent(int cId, string studentId)
        {
            

            var modules = (await _repo.GetModulesByCourseAsync(cId))
                .OrderBy(m => m.OrderIndex)
                .ToList();

            var studentProgress =
                await _progressRepo.GetProgressByStudentAsync(studentId, cId);

            var moduleDtos = new List<ModuleDto>();

            bool prevModuleCompleted = true;

            foreach (var module in modules)
            {
                var dto = _mapper.Map<ModuleDto>(module);

                dto.IsLocked = !prevModuleCompleted;

                var progress = studentProgress
                    .FirstOrDefault(p => p.ModuleId == module.Id);

                prevModuleCompleted = progress?.IsCompleted ?? false;

                moduleDtos.Add(dto);
            }

            

            return moduleDtos;
        }

        public async Task<bool?> UpdateModule(int mId, UpdateModuleDto update)
        {
            var module = await _repo.GetByIdAsync(mId);

            if (module == null)
                return null;

            _mapper.Map(update, module);

            await _repo.SaveChangesAsync();

          

            return true;
        }

      

 
    }
}