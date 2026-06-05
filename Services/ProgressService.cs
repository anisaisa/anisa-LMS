using anisa_lms.DTOs;
using anisa_lms.Interfaces.IRepository;
using anisa_lms.Interfaces.IService;
using anisa_lms.Models;
using AutoMapper;

namespace anisa_lms.Services
{
    public class ProgressService(
    IProgressRepository repo,
    IEnrollmentRepository enrollmentRepo,
    IEnrollmentAccessService enrollmentAccess,
    ICourseRepository courseRepo,
    IModuleRepository moduleRepo,
    IModuleService moduleService,
    IMapper mapper) : IProgressService
    {
        private readonly IProgressRepository _repo = repo;
        private readonly IMapper _mapper = mapper;
        private readonly IEnrollmentRepository _enrollmentRepo = enrollmentRepo;
        private readonly IEnrollmentAccessService _enrollmentAccess = enrollmentAccess;
        private readonly ICourseRepository _courseRepo = courseRepo;
        private readonly IModuleRepository _moduleRepo = moduleRepo;
        private readonly IModuleService _moduleService = moduleService;

        public async Task CreateProgress(CreateStudentModuleProgressDto create)
        {
            var targetModule = await _moduleRepo.GetByIdAsync(create.ModuleId)
                ?? throw new InvalidOperationException("Module not found.");

            await _enrollmentAccess.EnsureActiveEnrollmentAsync(create.StudentId!, targetModule.CourseId);

            var progress = _mapper.Map<StudentModuleProgress>(create);

            if (progress.IsCompleted && progress.CompletionDate == null)
            {
                progress.CompletionDate = DateTime.UtcNow;
            }

            await _repo.CreateAsync(progress);
            await _repo.SaveChangesAsync();

           

            if (progress.IsCompleted)
            {
                await CheckCourseCompletionAsync(progress.StudentId!, targetModule.CourseId);
            }
        }

        public async Task<bool?> DeleteProgress(int pId)
        {
            var progress = await _repo.GetByIdAsync(pId);
            if (progress == null) return null;

            if (progress.Module != null)
            {
                await _enrollmentAccess.EnsureActiveEnrollmentAsync(
                    progress.StudentId!,
                    progress.Module.CourseId);
            }

            _repo.DeleteAsync(progress);
            await _repo.SaveChangesAsync();

           

            return true;
        }

        public async Task<bool?> UpdateProgress(int pId, UpdateStudentModuleProgress update)
        {
            var progress = await _repo.GetByIdAsync(pId);

            if (progress == null)
                return null;

            if (progress.Module != null)
            {
                await _enrollmentAccess.EnsureActiveEnrollmentAsync(
                    progress.StudentId!,
                    progress.Module.CourseId);
            }

            var wasCompleted = progress.IsCompleted;

            _mapper.Map(update, progress);

            if (!wasCompleted && progress.IsCompleted)
            {
                progress.CompletionDate = DateTime.UtcNow;
            }

            await _repo.SaveChangesAsync();

          

            if (!wasCompleted && progress.IsCompleted && progress.Module != null)
            {
                await CheckCourseCompletionAsync(
                    progress.StudentId!,
                    progress.Module.CourseId);
            }

            return true;
        }

        public async Task<List<StudentModuleProgressDto>> GetProgressByStudentAsync(
      string studentId,
      int courseId)
        {
            var progress = await _repo.GetProgressByStudentAsync(
                studentId,
                courseId);

            return _mapper.Map<List<StudentModuleProgressDto>>(progress);
        }

        private async Task CheckCourseCompletionAsync(
        string studentId,
        int courseId)
        {
            var totalModules =
                await _courseRepo.GetModulesCountAsync(courseId);

            var completedModules =
                await _repo.GetCompletedModulesCountAsync(
                    studentId,
                    courseId);

            if (totalModules == 0)
                return;

            if (completedModules == totalModules)
            {
                var enrollment =
                    await _enrollmentRepo.GetByStudentAndCourseAsync(
                        studentId,
                        courseId);

                if (enrollment != null && enrollment.Status == StudentStatus.Active)
                {
                    enrollment.Status = StudentStatus.Completed;

                    await _enrollmentRepo.SaveChangesAsync();
                }
            }

        }
    }
}
