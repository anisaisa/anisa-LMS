using System.ComponentModel.DataAnnotations;

namespace anisa_lms.Models
{
    public class StudentModuleProgress
    {
        public int Id { get; set; }
        public string? StudentId { get; set; }
        public int ModuleId { get; set; }
        [Required]
        public bool IsCompleted { get; set; }
        public DateTime? CompletionDate { get; set; }

        public AppUser? Student { get; set; }
        public virtual Module? Module { get; set; }
    }
}
