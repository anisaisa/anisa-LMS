using Microsoft.AspNetCore.Identity;

namespace anisa_lms.Models
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = "";
    }
}
