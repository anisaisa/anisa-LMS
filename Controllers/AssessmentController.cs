using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;

using anisa_lms.DTOs;

using anisa_lms.Interfaces.IService;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using anisa_lms.Exceptions;

namespace anisa_lms.Controllers

{

    [ApiController]

    [Route("api/assessment")]

    [Authorize]

    public class AssessmentController(

        IAssessmentService assessmentService,

        IEnrollmentAccessService enrollmentAccess) : ControllerBase

    {

        private readonly IAssessmentService _assessmentService = assessmentService;

        private readonly IEnrollmentAccessService _enrollmentAccess = enrollmentAccess;



        [HttpGet("course/{cId:int}/upcoming")]

        [Authorize(Roles = "Admin,Instructor,Student")]

        public async Task<IActionResult> GetUpcoming([FromRoute] int cId)

        {

            if (User.IsInRole("Student"))

            {

                var studentId = GetCurrentUserId();

                if (studentId == null)

                {

                    return Unauthorized();

                }



                await _enrollmentAccess.EnsureActiveEnrollmentAsync(studentId, cId);

            }



            return Ok(await _assessmentService.GetUpcomingAssessments(cId));

        }



        [HttpGet("{aId:int}/results")]

        [Authorize(Roles = "Admin,Instructor")]

        public async Task<IActionResult> GetResults([FromRoute] int aId, [FromQuery] bool passed)

        {

            return Ok(await _assessmentService.GetResults(aId, passed));

        }



        [HttpPost]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> CreateAssessment(
     [FromBody] CreateAssessmentDto create)
        {
            try
            {
                var instructorId =
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var isAdmin = User.IsInRole("Admin");

                if (string.IsNullOrEmpty(instructorId))
                    return Unauthorized();

                await _assessmentService.CreateAssessment(
                    create,
                    instructorId,
                    isAdmin);

                return Created();
            }
            catch (EnrollmentAccessException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{aId:int}")]
            [Authorize(Roles = "Admin,Instructor")]
            public async Task<IActionResult> Update(
        [FromRoute] int aId,
        [FromBody] UpdateAssessmentDto update)
            {
                try
                {
                    var instructorId =
                        User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var isAdmin = User.IsInRole("Admin");

                    if (string.IsNullOrEmpty(instructorId))
                        return Unauthorized();

                    var result = await _assessmentService.UpdateAssessment(
                        aId,
                        update,
                        instructorId,
                          isAdmin);

                    if (result == null)
                    {
                        return NotFound(new
                        {
                            message = "Assessment with given ID does not exist."
                        });
                    }

                    return NoContent();
                }
                catch (EnrollmentAccessException ex)
                {
                    return BadRequest(new
                    {
                        message = ex.Message
                    });
                }
            }

            [HttpDelete("{aId:int}")]
            [Authorize(Roles = "Admin,Instructor")]
            public async Task<IActionResult> Delete([FromRoute] int aId)
            {
                try
                {
                    var instructorId =
                        User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    var isAdmin = User.IsInRole("Admin");

                    if (string.IsNullOrEmpty(instructorId))
                        return Unauthorized();

                    var result = await _assessmentService.DeleteAssessment(
                        aId,
                        instructorId,
                        isAdmin);

                    if (result == null)
                    {
                        return NotFound(new
                        {
                            message = "Assessment with given ID does not exist."
                        });
                    }

                    return NoContent();
                }
                catch (EnrollmentAccessException ex)
                {
                    return BadRequest(new
                    {
                        message = ex.Message
                    });
                }
            }
        


        private string? GetCurrentUserId()

        {

            return User.FindFirstValue(JwtRegisteredClaimNames.Sub)

                ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        }

    }

}


