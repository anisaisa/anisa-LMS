using anisa_lms.DTOs;

namespace anisa_lms.Interfaces.IService
{
    public interface ICourseService
    {
        public Task<PagedListDto<CourseDto>> GetAllCourses(CourseQueryParams query);
        public Task CreateCourse(CreateCourseDto create);
        public Task<bool?> UpdateCourse(int cId, UpdateCourseDto update);
        public Task<bool?> DeleteCourse(int cId);

        void InvalidateCourseCache();
    }
}
