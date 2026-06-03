using anisa_lms.DTOs;

namespace anisa_lms.Interfaces.IRepository
{
    public interface IDashboardRepository
    {
        public Task<AdminDashboardDto> GetAdminDashboardAsync();
        public Task<InstructorDashboardDto> GetInstructorDashboardAsync(string instructorId);
        public Task<StudentDashboardDto> GetStudentDashboardAsync(string studentId);
    }
}
