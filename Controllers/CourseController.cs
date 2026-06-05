using anisa_lms.DTOs;
using anisa_lms.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace anisa_lms.Controllers
{
    [ApiController]
    [Route("api/course")]
    public class CourseController(ICourseService courseService) : ControllerBase
    {
        private readonly ICourseService _courseService = courseService;

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] CourseQueryParams query)
        {
            return Ok(await _courseService.GetAllCourses(query));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCourseDto create)
        {
            await _courseService.CreateCourse(create);

            return Ok(new { message = "Course created successfully." });
        }

        [HttpPut("{cId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromRoute] int cId, [FromBody] UpdateCourseDto update)
        {
            var result = await _courseService.UpdateCourse(cId, update);
            if (result == null) return NotFound(new { message = "Course with given ID does not exist." });

            return NoContent();
        }

        [HttpDelete("{cId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([FromRoute] int cId)
        {
            var result = await _courseService.DeleteCourse(cId);
            if (result == null) return NotFound(new { message = "Course with given ID does not exist." });

            return NoContent();
        }
    }
}
