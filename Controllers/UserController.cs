using anisa_lms.DTOs;
using anisa_lms.Interfaces.IService;
using anisa_lms.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace anisa_lms.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
        ITokenService tokenService, RoleManager<IdentityRole> roleManager) : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly SignInManager<AppUser> _signInManager = signInManager;
        private readonly ITokenService _tokenService = tokenService;

        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(new { message = "Email is not valid." });

                var user = await _userManager.FindByEmailAsync(login.Email);
                if (user == null) return Unauthorized(new { message = "Incorrect login credentials." });

                var result = await _signInManager.CheckPasswordSignInAsync(user, login.Password, false);
                if (!result.Succeeded) return Unauthorized(new { message = "Incorrect login credentials." });

                var roles = await _userManager.GetRolesAsync(user); // All roles assigned to this user. Returns IList
                var role = roles.FirstOrDefault() ?? "Student";

                var token = _tokenService.GenerateJwtToken(user, role);

                Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                return Ok(new UserDto
                {
                    Id = user.Id,
                    Role = role,
                    FullName = user.FullName,
                    Token = token,
                });
            }
            catch
            {
                return StatusCode(500, new { message = "An internal server error occurred." });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(new { message = "Email is not valid." });

                AppUser user = new()
                {
                    FullName = register.FullName,
                    Email = register.Email,
                    UserName = register.Email,
                };

                var result = await _userManager.CreateAsync(user, register.Password);
                if (!result.Succeeded) return BadRequest(result.Errors);
                await _userManager.AddToRoleAsync(user, "Student");

                return Ok(new
                {
                    message = "User registered successfully.",
                    userId = user.Id
                });
            }
            catch
            {
                return StatusCode(500, new { message = "An internal server error occurred." });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt", new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
            });

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDTO dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);

            if (user == null)
            {
                return NotFound("User does not exist.");
            }

            if (!await _roleManager.RoleExistsAsync(dto.RoleName))
            {
                return BadRequest("Role does not exist.");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            var result = await _userManager.AddToRoleAsync(user, dto.RoleName);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new
            {
                message = "Role updated successfully."
            });
        }

        [Authorize(Roles = "Admin,Instructor")]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    Role = roles.FirstOrDefault() ?? "Student",
                });
            }

            return Ok(result);
        }
        [Authorize(Roles = "Admin,Instructor")]
        [HttpGet("students")]
        public async Task<IActionResult> GetStudents()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<object>();

            foreach (var user in users)
            {
                var role = (await _userManager.GetRolesAsync(user))
                    .FirstOrDefault();

                if (role == "Student")
                {
                    result.Add(new
                    {
                        user.Id,
                        user.FullName,
                        user.Email,
                        Role = role
                    });
                }
            }

            return Ok(result);
        }
    }
}
    