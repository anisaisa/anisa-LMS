using anisa_lms.Exceptions;
using anisa_lms.Interfaces.IRepository;
using anisa_lms.Interfaces.IService;
using anisa_lms.Models;

namespace anisa_lms.Services;

public class EnrollmentAccessService(IEnrollmentRepository enrollmentRepo) : IEnrollmentAccessService
{

    //just for active users
    public async Task EnsureActiveEnrollmentAsync(string studentId, int courseId)
    {
        if (string.IsNullOrWhiteSpace(studentId))
        {
            throw new EnrollmentAccessException("Student id is required.");
        }

        var enrollment = await enrollmentRepo.GetByStudentAndCourseAsync(studentId, courseId);

        if (enrollment == null)
        {
            throw new EnrollmentAccessException("You are not enrolled in this course.");
        }

        if (enrollment.Status != StudentStatus.Active)
        {
            throw new EnrollmentAccessException("Your enrollment in this course is not active.");
        }
    }

    public async Task EnsureProgressWriteAllowedAsync(string studentId, int courseId, bool requireActive)
    {
        if (string.IsNullOrWhiteSpace(studentId))
        {
            throw new EnrollmentAccessException("Student id is required.");
        }

        var enrollment = await enrollmentRepo.GetByStudentAndCourseAsync(studentId, courseId);

        if (enrollment == null)
        {
            throw new EnrollmentAccessException("This student is not enrolled in this course.");
        }

        if (requireActive && enrollment.Status != StudentStatus.Active)
        {
            throw new EnrollmentAccessException("Your enrollment in this course is not active.");
        }
    }
}
