namespace anisa_lms.DTOs
{
    public class StudentModuleProgressBaseDto
    {
        public bool IsCompleted { get; set; }
        public DateTime? CompletionDate { get; set; }
    }

    public class CreateStudentModuleProgressDto : StudentModuleProgressBaseDto
    {
        public string? StudentId { get; set; }
        public int ModuleId { get; set; }
    }

    public class UpdateStudentModuleProgress : StudentModuleProgressBaseDto { }

    public class StudentModuleProgressDto : StudentModuleProgressBaseDto
    {
        public int Id { get; set; }

        public int ModuleId { get; set; }

        public string StudentFullName { get; set; } = "";
    }
}
