using anisa_lms.Models;

namespace anisa_lms.Interfaces.IService
{
    public interface ITokenService
    {
        public string GenerateJwtToken(AppUser user, string role);
    }
}
