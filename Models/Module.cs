using System.ComponentModel.DataAnnotations;

namespace anisa_lms.Models
{
    public class Module
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        [Required]
        public string Title { get; set; } = "";
        [Required]
        public string Content { get; set; } = "";
        public int OrderIndex { get; set; }

        public Course? Course { get; set; }
    }
}
