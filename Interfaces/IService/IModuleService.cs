using anisa_lms.DTOs;

namespace anisa_lms.Interfaces.IService
{
    public interface IModuleService
    {
        Task<List<ModuleDto>> GetModulesForStudent(int cId, string studentId);
        Task CreateModule(CreateModuleDto create);
        Task<bool?> UpdateModule(int mId, UpdateModuleDto update);
        Task<bool?> DeleteModule(int mId);

        
    }
}
