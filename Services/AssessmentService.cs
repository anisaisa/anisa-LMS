using anisa_lms.DTOs;
using anisa_lms.Exceptions;
using anisa_lms.Interfaces.IRepository;
using anisa_lms.Interfaces.IService;
using anisa_lms.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;


namespace anisa_lms.Services
{
    public class AssessmentService(
        IAssessmentRepository repo,
        ICourseService courseService,
        IMapper mapper,
        IMemoryCache cache,
        ICourseRepository courseRepo) : IAssessmentService
    {
        private readonly IAssessmentRepository _repo = repo;
        private readonly ICourseService _courseService = courseService;
        private readonly IMapper _mapper = mapper;
        private readonly IMemoryCache _cache = cache;
        private readonly ICourseRepository _courseRepo = courseRepo;

        private static readonly MemoryCacheEntryOptions ShortLived =
            new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        private const string UpcomingKeyPrefix = "upcoming_assessments_";
        private const string ResultsKeyPrefix = "assessment_results_";

        public async Task CreateAssessment(
    CreateAssessmentDto create,
    string instructorId, bool isAdmin)
        {
            var course = await _courseRepo.GetByIdAsync(create.CourseId);

            if (course == null)
                throw new Exception("Course not found");

            if (!isAdmin&&course.InstructorId != instructorId)
                throw new Exception(
                    "You can only create assessments for your own courses");

            var assessment = _mapper.Map<Assessment>(create);

            await _repo.CreateAsync(assessment);
            await _repo.SaveChangesAsync();

            _courseService.InvalidateCourseCache();
            _cache.Remove($"{UpcomingKeyPrefix}{assessment.CourseId}");
        }
        public async Task<bool?> DeleteAssessment(
    int id,
    string instructorId,bool isAdmin)
        {
            var assessment = await _repo.GetByIdAsync(id);

            if (assessment == null)
                return null;

            var course = await _courseRepo.GetByIdAsync(assessment.CourseId);

            if (course == null)
                throw new Exception("Course not found");

            if (!isAdmin&& course.InstructorId != instructorId)
                throw new EnrollmentAccessException(
    "You can only edit assessments for your own courses");

            _repo.DeleteAsync(assessment);
            await _repo.SaveChangesAsync();

            _courseService.InvalidateCourseCache();
            _cache.Remove($"{UpcomingKeyPrefix}{assessment.CourseId}");
            _cache.Remove($"{ResultsKeyPrefix}{id}_True");
            _cache.Remove($"{ResultsKeyPrefix}{id}_False");

            return true;
        }

        public async Task<List<AssessmentScoreDto>> GetResults(int aId, bool passed)
        {
            var key = $"{ResultsKeyPrefix}{aId}_{passed}";

            if (_cache.TryGetValue(key, out List<AssessmentScoreDto>? cached))
                return cached!;

            var query = _repo.GetAssessmentScores(aId);

            List<AssessmentScoreDto> result;

            if (passed)
            {
                result = await query
                    .Include(s => s.Student)
                    .Include(s => s.Assessment)
                    .Where(s => s.Score >= s.Assessment.PassRequirement)
                    .ProjectTo<AssessmentScoreDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();
            }
            else
            {
                result = await query
                    .Include(s => s.Student)
                    .Include(s => s.Assessment)
                    .Where(s => s.Score < s.Assessment.PassRequirement)
                    .ProjectTo<AssessmentScoreDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();
            }

            _cache.Set(key, result, ShortLived);

            return result;
        }

        public async Task<List<AssessmentDto>> GetUpcomingAssessments(int cId)
        {
            var key = $"{UpcomingKeyPrefix}{cId}";

            if (_cache.TryGetValue(key, out List<AssessmentDto>? cached))
                return cached!;

            var result = await _repo.GetUpcomingQueryable(cId)
                .ProjectTo<AssessmentDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            _cache.Set(key, result, ShortLived);

            return result;
        }

        public async Task<bool?> UpdateAssessment(
    int id,
    UpdateAssessmentDto update,
    string instructorId,bool isAdmin)
        {
            var assessment = await _repo.GetByIdAsync(id);

            if (assessment == null)
                return null;

            var course = await _courseRepo.GetByIdAsync(assessment.CourseId);

            if (course == null)
                throw new Exception("Course not found");

            if (!isAdmin&&course.InstructorId != instructorId)
                throw new EnrollmentAccessException(
    "You can only edit assessments for your own courses");

            _mapper.Map(update, assessment);

            await _repo.SaveChangesAsync();

            _courseService.InvalidateCourseCache(); //opt
            _cache.Remove($"{UpcomingKeyPrefix}{assessment.CourseId}"); //opt
            _cache.Remove($"{ResultsKeyPrefix}{id}_True");
            _cache.Remove($"{ResultsKeyPrefix}{id}_False");

            return true;
        }
    }
}