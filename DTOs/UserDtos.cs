using System.ComponentModel.DataAnnotations;

namespace anisa_lms.DTOs
{
    public class UserAuthBaseDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }
    }

    public class LoginDto : UserAuthBaseDto { };

    public class RegisterDto : UserAuthBaseDto
    {
        [Required]
        public required string FullName { get; set; }
    };

    public class UserDto
    {
        public string Id { get; set; } = "";
        public string Role { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Token { get; set; } = "";
    }
}
