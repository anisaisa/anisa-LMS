using anisa_lms.DTOs;
using anisa_lms.Interfaces.IRepository;
using anisa_lms.Interfaces.IService;
using anisa_lms.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace anisa_lms.Services
{
    public class CourseService(
        ICourseRepository repo,
        IMapper mapper,
        IMemoryCache cache) : ICourseService
    {
        private readonly ICourseRepository _repo = repo;
        private readonly IMapper _mapper = mapper;
        private readonly IMemoryCache _cache = cache;

        private const string CourseKeyPrefix = "courses_";
        private static int _listCacheVersion;

        private static readonly MemoryCacheEntryOptions ShortLived =
            new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        public async Task CreateCourse(CreateCourseDto create)
        {
            var course = _mapper.Map<Course>(create);

            await _repo.CreateAsync(course);
            await _repo.SaveChangesAsync();

            InvalidateCourseCache();
        }

        public async Task<bool?> DeleteCourse(int cId)
        {
            var course = await _repo.GetByIdAsync(cId);

            if (course == null)
                return null;

            _repo.Delete(course);
            await _repo.SaveChangesAsync();

            InvalidateCourseCache();

            return true;
        }

        public async Task<PagedListDto<CourseDto>> GetAllCourses(CourseQueryParams query)
        {
            var key =
                $"{CourseKeyPrefix}" +
                $"{Volatile.Read(ref _listCacheVersion)}_" +
                $"{query.Title}_{query.Page}_{query.PageSize}";

            if (_cache.TryGetValue(key, out PagedListDto<CourseDto>? cached))
                return cached!;

            var courses = _repo.GetAllQueryable();

            if (!string.IsNullOrWhiteSpace(query.Title))
                courses = courses.Where(c => c.Title.Contains(query.Title));

            var totalCount = await courses.CountAsync();

            var pageItems = await courses
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var result = new PagedListDto<CourseDto>
            {
                Items = _mapper.Map<List<CourseDto>>(pageItems),
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize
            };

            _cache.Set(key, result, ShortLived);

            return result;
        }

        public async Task<bool?> UpdateCourse(int cId, UpdateCourseDto update)
        {
            var course = await _repo.GetByIdAsync(cId);

            if (course == null)
                return null;

            _mapper.Map(update, course);

            await _repo.SaveChangesAsync();

            InvalidateCourseCache();

            return true;
        }

        //is to force all cached course lists to become outdated whenever a course is added, edited, or deleted.
        public void InvalidateCourseCache()
        {
            Interlocked.Increment(ref _listCacheVersion);
        }
    }
}