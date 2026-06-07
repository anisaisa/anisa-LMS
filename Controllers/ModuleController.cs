using anisa_lms.DTOs;
using anisa_lms.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace anisa_lms.Controllers
{
    [ApiController]
    [Route("api/module")]
    public class ModuleController(
        IModuleService moduleService,
        IEnrollmentAccessService enrollmentAccess) : ControllerBase
    {
        [HttpGet("~/api/course/{cId:int}/module")]
        [Authorize(Roles = "Admin,Instructor,Student")]
        public async Task<IActionResult> GetModulesForStudent(
            [FromRoute] int cId,
            [FromQuery] string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
            {
                return BadRequest(new
                {
                    message = "studentId query parameter is required."
                });
            }

            if (User.IsInRole("Student"))
            {
                await enrollmentAccess.EnsureActiveEnrollmentAsync(
                    studentId,
                    cId);
            }

            return Ok(
                await moduleService.GetModulesForStudent(
                    cId,
                    studentId));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> Create(
            [FromBody] CreateModuleDto create)
        {
            await moduleService.CreateModule(create);

            return Ok(new
            {
                message = "Module created successfully."
            });
        }

        [HttpPut("{mId:int}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> Update(
            [FromRoute] int mId,
            [FromBody] UpdateModuleDto update)
        {
            var result = await moduleService.UpdateModule(
                mId,
                update);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Module with given ID does not exist."
                });
            }

            return NoContent();
        }

        [HttpDelete("{mId:int}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> Delete(
            [FromRoute] int mId)
        {
            var result = await moduleService.DeleteModule(mId);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Module with given ID does not exist."
                });
            }

            return NoContent();
        }
    }
}