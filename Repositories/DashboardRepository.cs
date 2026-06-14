using anisa_lms.Data;
using anisa_lms.DTOs;
using anisa_lms.Interfaces.IRepository;
using anisa_lms.Models;
using Microsoft.EntityFrameworkCore;

namespace anisa_lms.Repositories
{
    public class DashboardRepository(AppDbContext context) : IDashboardRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<AdminDashboardDto> GetAdminDashboardAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalCourses = await _context.Courses.CountAsync();

            var popularCourses = await _context.Courses
                .AsNoTracking()
                .Select(c => new DashboardCourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    Status = c.Status.ToString(),
                    CreatedAt = c.CreatedAt,
                    MaxEnrollments = c.MaxEnrollments,
                    EnrollmentCount = c.Enrollments.Count(),
                    InstructorFullName = c.Instructor != null
                        ? c.Instructor.FullName
                        : ""
                })
                .OrderByDescending(c => c.EnrollmentCount)
                .Take(5)
                .ToListAsync();

            var recentCourses = await _context.Courses
                .AsNoTracking()
                .Select(c => new DashboardCourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    Status = c.Status.ToString(),
                    CreatedAt = c.CreatedAt,
                    MaxEnrollments = c.MaxEnrollments,
                    EnrollmentCount = c.Enrollments.Count(),
                    InstructorFullName = c.Instructor != null
                        ? c.Instructor.FullName
                        : ""
                })
                .OrderByDescending(c => c.CreatedAt)
                .Take(5)
                .ToListAsync();

            return new AdminDashboardDto
            {
                TotalUsers = totalUsers,
                TotalCourses = totalCourses,
                PopularCourses = popularCourses,
                RecentCourses = recentCourses
            };
        }


        public async Task<InstructorDashboardDto> GetInstructorDashboardAsync(string instructorId)
        {
            var query = _context.Courses.AsNoTracking().Where(c => c.InstructorId == instructorId);

            var myCourses = await query.ToListAsync();
            var recentCourses = await query.OrderByDescending(c => c.CreatedAt).Take(5).ToListAsync();
            var assessments = await query.Include(c => c.Assessments).Select(c => c.Assessments).ToListAsync();
            //var studentsEnrolled = await _context.Enrollments.CountAsync();
            var studentsEnrolled = await _context.Enrollments
    .CountAsync(e => e.Course.InstructorId == instructorId);

            return new InstructorDashboardDto
            {
                MyCourses = myCourses,
                RecentCourses = recentCourses,
                Assessments = assessments,
                StudentsEnrolled = studentsEnrolled,
            };
        }

        public async Task<StudentDashboardDto> GetStudentDashboardAsync(string studentId)
        {
            var coursesInProgress = await _context.Enrollments.AsNoTracking()
                .Where(e => e.StudentId == studentId && e.Status == StudentStatus.Active)
                .Select(e => e.Course)
                .ToListAsync();
            var completedAssessments = await _context.AssessmentScores.Where(a => a.StudentId == studentId).CountAsync();
            var totalEnrollments = await _context.Enrollments.Where(e => e.StudentId == studentId).CountAsync();
            var modulesCompleted = await _context.ModuleProgresses.Where(e => e.StudentId == studentId && e.IsCompleted == true).CountAsync();

            return new StudentDashboardDto
            {
                CoursesInProgress = coursesInProgress,
                CompletedAssessments = completedAssessments,
                TotalEnrollments = totalEnrollments,
                ModulesCompleted = modulesCompleted,
            };
        }
    }
}
