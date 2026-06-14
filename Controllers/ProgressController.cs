using anisa_lms.DTOs;

using anisa_lms.Interfaces.IService;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;



namespace anisa_lms.Controllers

{

    [ApiController]

    [Route("api/progress")]

    [Authorize(Roles = "Admin,Instructor,Student")]

    public class ProgressController(IProgressService progressService) : ControllerBase

    {

        private readonly IProgressService _progressService = progressService;



        [HttpPost]

        public async Task<IActionResult> Create([FromBody] CreateStudentModuleProgressDto create)

        {
            var requireActiveEnrollment = User.IsInRole("Student");

            await _progressService.CreateProgress(create, requireActiveEnrollment);



            return NoContent();

        }



        [HttpPut("{pId:int}")]

        public async Task<IActionResult> Update([FromRoute] int pId, [FromBody] UpdateStudentModuleProgress update)

        {

            var requireActiveEnrollment = User.IsInRole("Student");

            var result = await _progressService.UpdateProgress(pId, update, requireActiveEnrollment);

            if (result == null) return NotFound(new { message = "Progress with given ID does not exist." });



            return NoContent();

        }


        [HttpGet]

        public async Task<IActionResult> GetProgress( [FromQuery] string studentId, [FromQuery] int courseId)

        {

            var result = await _progressService

                .GetProgressByStudentAsync(studentId, courseId);



            return Ok(result);

        }



        [HttpDelete("{pId:int}")]

        public async Task<IActionResult> Delete([FromRoute] int pId)

        {

            var requireActiveEnrollment = User.IsInRole("Student");

            var result = await _progressService.DeleteProgress(pId, requireActiveEnrollment);

            if (result == null) return NotFound(new { message = "Progress with given ID does not exist." });



            return NoContent();

        }

    }

}


