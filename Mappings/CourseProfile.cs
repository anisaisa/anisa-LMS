using anisa_lms.DTOs;
using anisa_lms.Models;
using AutoMapper;

namespace anisa_lms.Mappings
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<Course, CourseDto>()
                .ForMember(
                    dest => dest.InstructorFullName,
                    opt => opt.MapFrom(src => src.Instructor != null ? src.Instructor.FullName : ""))
                .ForMember(dest => dest.Enrollments, opt => opt.MapFrom(src => src.Enrollments))
                .ForMember(dest => dest.Modules, opt => opt.MapFrom(src => src.Modules))
                .ForMember(dest => dest.Assessments, opt => opt.MapFrom(src => src.Assessments));

            CreateMap<CreateCourseDto, Course>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Instructor, opt => opt.Ignore())
                .ForMember(dest => dest.Enrollments, opt => opt.Ignore())
                .ForMember(dest => dest.Modules, opt => opt.Ignore())
                .ForMember(dest => dest.Assessments, opt => opt.Ignore());

            CreateMap<UpdateCourseDto, Course>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Instructor, opt => opt.Ignore())
                .ForMember(dest => dest.Enrollments, opt => opt.Ignore())
                .ForMember(dest => dest.Modules, opt => opt.Ignore())
                .ForMember(dest => dest.Assessments, opt => opt.Ignore());
        }
    }
}
