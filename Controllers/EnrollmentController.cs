using anisa_lms.DTOs;
using anisa_lms.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace anisa_lms.Controllers
{
    [ApiController]
    [Route("api/enrollment")]
    [Authorize(Roles = "Admin,Instructor")]
    public class EnrollmentController(IEnrollmentService enrollmentService) : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService = enrollmentService;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEnrollmentDto create)
        {
            await _enrollmentService.CreateEnrollment(create);

            return Ok(new { message = "Student enrolled successfully." });
        }

        [HttpPut("{eId:int}")]
        public async Task<IActionResult> Update([FromRoute] int eId, [FromBody] UpdateEnrollmentDto update)
        {
            var enrollment = await _enrollmentService.UpdateEnrollment(eId, update);
            if (enrollment == null) return NotFound(new { message = "Enrollment with given ID does not exist." });

            return NoContent();
        }

        [HttpGet("{eId:int}")]
        public async Task<IActionResult> GetById(int eId)
        {
            var enrollment = await _enrollmentService.GetByIdAsync(eId);

            if (enrollment == null)
                return NotFound();

            return Ok(enrollment);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var enrollments = await _enrollmentService.GetAllAsync();

            return Ok(enrollments);
        }
        [HttpDelete("{eId:int}")]
        public async Task<IActionResult> Delete([FromRoute] int eId)
        {
            var enrollment = await _enrollmentService.DeleteEnrollment(eId);
            if (enrollment == null) return NotFound(new { message = "Enrollment with given ID does not exist." });

            return NoContent();
        }
    }
}
