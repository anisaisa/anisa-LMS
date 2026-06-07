using anisa_lms.Interfaces.IRepository;
using anisa_lms.Interfaces.IService;
using anisa_lms.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace anisa_lms.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController(UserManager<AppUser> user, IDashboardRepository repo) : ControllerBase
    {
        private readonly UserManager<AppUser> _user = user;
        private readonly IDashboardRepository _repo = repo;

        [HttpGet]
        public async Task<IActionResult> Dashboard([FromQuery] string email)
        {
            var user = await _user.FindByEmailAsync(email);
            if (user == null) return NotFound(new { message = "User with given email does not exist." });

            var roles = await _user.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Student";

            if (role == "Admin") return Ok(await _repo.GetAdminDashboardAsync());
            else if (role == "Instructor") return Ok(await _repo.GetInstructorDashboardAsync(user.Id));
            else return Ok(await _repo.GetStudentDashboardAsync(user.Id));
        }

    }
}
