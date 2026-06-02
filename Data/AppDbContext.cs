using anisa_lms.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace anisa_lms.Data
{
    public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<AssessmentScore> AssessmentScores { get; set; }
        public DbSet<StudentModuleProgress> ModuleProgresses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Enrollment>()
                .HasIndex(e => new { e.CourseId, e.StudentId })
                .IsUnique();

            builder.Entity<StudentModuleProgress>()
        .HasIndex(p => new { p.StudentId, p.ModuleId })
        .IsUnique();
        }
    }
}
