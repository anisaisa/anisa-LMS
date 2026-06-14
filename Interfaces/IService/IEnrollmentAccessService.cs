namespace anisa_lms.Interfaces.IService;

public interface IEnrollmentAccessService
{
    Task EnsureActiveEnrollmentAsync(string studentId, int courseId);

    /// <param name="requireActive">When true (students), enrollment must be Active. When false (staff), any existing enrollment is enough.</param>
    Task EnsureProgressWriteAllowedAsync(string studentId, int courseId, bool requireActive);
}
