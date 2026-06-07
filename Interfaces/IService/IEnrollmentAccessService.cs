namespace anisa_lms.Interfaces.IService;

public interface IEnrollmentAccessService
{
    Task EnsureActiveEnrollmentAsync(string studentId, int courseId);
}
