using anisa_lms.DTOs;
using anisa_lms.Interfaces.IRepository;
using anisa_lms.Interfaces.IService;
using anisa_lms.Models;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;

namespace anisa_lms.Services
{
    public class ProgressService(
    IProgressRepository repo,
    IEnrollmentRepository enrollmentRepo,
    IEnrollmentAccessService enrollmentAccess,
    ICourseRepository courseRepo,
    IModuleRepository moduleRepo,
    IModuleService moduleService,
    IMapper mapper,
        IMemoryCache cache) : IProgressService
    {
        private readonly IProgressRepository _repo = repo;
        private readonly IMapper _mapper = mapper;
        private readonly IEnrollmentRepository _enrollmentRepo = enrollmentRepo;
        private readonly IEnrollmentAccessService _enrollmentAccess = enrollmentAccess;
        private readonly ICourseRepository _courseRepo = courseRepo;
        private readonly IModuleRepository _moduleRepo = moduleRepo;

        private readonly IMemoryCache _cache = cache;

        private const string StudentProgressKey = "student_progress";
        private const string CourseCompletionKey = "course_completion";

        private static readonly MemoryCacheEntryOptions CacheOptions =
            new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        public async Task CreateProgress(CreateStudentModuleProgressDto create)
        {
            var targetModule = await _moduleRepo.GetByIdAsync(create.ModuleId)
                ?? throw new InvalidOperationException("Module not found.");

            await _enrollmentAccess.EnsureActiveEnrollmentAsync(create.StudentId!, targetModule.CourseId);

            var existing = await _repo.GetByStudentAndModuleAsync(create.StudentId!, create.ModuleId);
            if (existing != null)
            {
                var wasCompleted = existing.IsCompleted;
                existing.IsCompleted = create.IsCompleted;

                if (!wasCompleted && existing.IsCompleted)
                {
                    existing.CompletionDate = create.CompletionDate ?? DateTime.UtcNow;
                }

                await _repo.SaveChangesAsync();
                InvalidateProgressCache(create.StudentId!, create.ModuleId, targetModule.CourseId);

                if (!wasCompleted && existing.IsCompleted)
                {
                    await CheckCourseCompletionAsync(create.StudentId!, targetModule.CourseId);
                }

                return;
            }

            var progress = _mapper.Map<StudentModuleProgress>(create);

            if (progress.IsCompleted && progress.CompletionDate == null)
            {
                progress.CompletionDate = DateTime.UtcNow;
            }

            await _repo.CreateAsync(progress);
            await _repo.SaveChangesAsync();
            InvalidateProgressCache(create.StudentId!, create.ModuleId, targetModule.CourseId);

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

            var courseId = progress.Module?.CourseId;

            _repo.DeleteAsync(progress);
            await _repo.SaveChangesAsync();

            if (courseId != null)
            {
                InvalidateProgressCache(progress.StudentId!, progress.ModuleId, courseId.Value);
            }

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

            if (progress.Module != null)
            {
                InvalidateProgressCache(
                    progress.StudentId!,
                    progress.ModuleId,
                    progress.Module.CourseId);
            }


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
            var cacheKey = $"{StudentProgressKey}_{studentId}_{courseId}";

            if (_cache.TryGetValue(cacheKey, out List<StudentModuleProgressDto>? cached))
                return cached!;

            var progress = await _repo.GetProgressByStudentAsync(
                studentId,
                courseId);

            var result = _mapper.Map<List<StudentModuleProgressDto>>(progress);

            _cache.Set(cacheKey, result, CacheOptions);

            return result;
        }

        private void InvalidateProgressCache(string studentId, int moduleId, int courseId)
        {
            _cache.Remove($"{StudentProgressKey}_{studentId}_{courseId}");
            _cache.Remove($"{StudentProgressKey}_{studentId}_{moduleId}");
            _cache.Remove($"{CourseCompletionKey}_{studentId}");
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
