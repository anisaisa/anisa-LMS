using System.ComponentModel.DataAnnotations;

namespace anisa_lms.DTOs
{
    public class AssignRoleDTO
    {
        [Required]
        public string UserId { get; set; } = "";

        [Required]
        public string RoleName { get; set; } = "";
    }
}
