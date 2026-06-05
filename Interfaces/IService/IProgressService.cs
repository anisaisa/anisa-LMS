using anisa_lms.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace anisa_lms.Interfaces.IService
{
    public interface IProgressService
    {
        public Task CreateProgress(CreateStudentModuleProgressDto create);
        public Task<bool?> UpdateProgress(int pId, UpdateStudentModuleProgress update);
        public Task<bool?> DeleteProgress(int pId);

        Task<List<StudentModuleProgressDto>> GetProgressByStudentAsync(
      string studentId,
      int courseId);
    }
}
