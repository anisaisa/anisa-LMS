using anisa_lms.DTOs;
using anisa_lms.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace anisa_lms.Controllers
{
    [ApiController]
    [Route("api/assessment-score")]
    [Authorize(Roles = "Admin,Instructor")]
    public class AssessmentScoreController(IAssessmentScoreService service) : ControllerBase
    {
        private readonly IAssessmentScoreService _service = service;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAssessmentScoreDto create)
        {
            await _service.CreateAssessmentScore(create);

            return Ok(new { message = "Assessment Score created successfully" });
        }

        [HttpPut("{asId:int}")]
        public async Task<IActionResult> Update([FromRoute] int asId, [FromBody] UpdateAssessmentScoreDto update)
        {
            var result = await _service.UpdateAssessmentScore(asId, update);
            if (result == null) return NotFound(new { message = "Assessment Score with given ID does not exist." });

            return NoContent();
        }

        [HttpDelete("{asId:int}")]
        public async Task<IActionResult> Delete([FromRoute] int asId)
        {
            var result = await _service.DeleteAssessmentScore(asId);
            if (result == null) return NotFound(new { message = "Assessment Score with given ID does not exist." });

            return NoContent();
        }
    }
}
