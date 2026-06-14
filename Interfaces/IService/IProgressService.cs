using anisa_lms.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace anisa_lms.Interfaces.IService
{
    public interface IProgressService
    {
        public Task CreateProgress(CreateStudentModuleProgressDto create, bool requireActiveEnrollment);
        public Task<bool?> UpdateProgress(int pId, UpdateStudentModuleProgress update, bool requireActiveEnrollment);
        public Task<bool?> DeleteProgress(int pId, bool requireActiveEnrollment);

        Task<List<StudentModuleProgressDto>> GetProgressByStudentAsync(
      string studentId,
      int courseId);
    }
}
