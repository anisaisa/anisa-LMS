using Microsoft.AspNetCore.Identity;

namespace anisa_lms.Data
{
    public static class RoleSeeder
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = ["Instructor", "Student", "Admin"];

            foreach (var role in roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);

                if (!roleExists) await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
