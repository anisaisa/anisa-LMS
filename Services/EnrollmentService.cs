using anisa_lms.Data;
using anisa_lms.DTOs;
using anisa_lms.Exceptions;
using anisa_lms.Interfaces.IRepository;
using anisa_lms.Interfaces.IService;
using anisa_lms.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace anisa_lms.Services
{
    public class EnrollmentService(
    IEnrollmentRepository repo,
    ICourseRepository courseRepo,
    IMapper mapper,
    IMemoryCache cache
        ) : IEnrollmentService
    {
        private readonly IEnrollmentRepository _repo = repo;
        private readonly ICourseRepository _courseRepo = courseRepo;
        private readonly IMapper _mapper = mapper;
        private readonly IMemoryCache _cache = cache;

        private static readonly MemoryCacheEntryOptions ShortLived =
            new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        private const string EnrollmentKeyPrefix = "enrollment_";
        private const string AllEnrollmentsKey = "all_enrollments";


        public async Task CreateEnrollment(CreateEnrollmentDto create)
        {
            var course = await _courseRepo.GetByIdAsync(create.CourseId) ?? throw new Exception("Course not found");
            var enrollmentsCount = await _courseRepo.GetEnrollmentsCountAsync(create.CourseId);

            if (course.MaxEnrollments <= enrollmentsCount)
                throw new Exception("Course is full. You cannot enroll anymore students");

            var enrollment = _mapper.Map<Enrollment>(create);

            await _repo.CreateAsync(enrollment);
            await _repo.SaveChangesAsync();

            _cache.Remove(AllEnrollmentsKey);
        }

        public async Task<EnrollmentDto?> GetByIdAsync(int eId)
        {
            var key = $"{EnrollmentKeyPrefix}{eId}";

            if (_cache.TryGetValue(key, out EnrollmentDto? cached))
                return cached;

            var enrollment = await _repo.GetByIdAsync(eId);

            if (enrollment == null)
                return null;

            var result = _mapper.Map<EnrollmentDto>(enrollment);

            _cache.Set(key, result, ShortLived);

            return result;
        }
        public async Task<List<EnrollmentDto>> GetAllAsync()
        {
            if (_cache.TryGetValue(
                AllEnrollmentsKey,
                out List<EnrollmentDto>? cached))
            {
                return cached!;
            }

            var enrollments = await _repo.GetAllAsync();

            var result = _mapper.Map<List<EnrollmentDto>>(enrollments);

            _cache.Set(
                AllEnrollmentsKey,
                result,
                ShortLived);

            return result;
        }
        public async Task<bool?> DeleteEnrollment(int eId)
        {
            var enrollment = await _repo.GetByIdAsync(eId);
            if (enrollment == null) return null;

            _repo.DeleteAsync(enrollment);
            await _repo.SaveChangesAsync();
            _cache.Remove($"{EnrollmentKeyPrefix}{eId}");
            _cache.Remove(AllEnrollmentsKey);

            return true;
        }

        public async Task<bool?> UpdateEnrollment(int eId, UpdateEnrollmentDto update)
        {
            var enrollment = await _repo.GetByIdAsync(eId);
            if (enrollment == null) return null;

            _mapper.Map(update, enrollment);
            await _repo.SaveChangesAsync();

            _cache.Remove($"{EnrollmentKeyPrefix}{eId}");
            _cache.Remove(AllEnrollmentsKey);

            return true;
        }


       
    }
}
