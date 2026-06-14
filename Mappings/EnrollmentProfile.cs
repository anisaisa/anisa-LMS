using anisa_lms.DTOs;
using anisa_lms.Models;
using AutoMapper;

namespace anisa_lms.Mappings
{
    public class EnrollmentProfile : Profile
    {
        public EnrollmentProfile()
        {
            CreateMap<Enrollment, EnrollmentDto>()
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
                .ForMember(
                    dest => dest.StudentFullName,
                    opt => opt.MapFrom(src => src.Student != null ? src.Student.FullName : ""))
                .ForMember(
                    dest => dest.CourseTitle,
                    opt => opt.MapFrom(src => src.Course != null ? src.Course.Title : ""));

            CreateMap<CreateEnrollmentDto, Enrollment>()
                .ForMember(dest => dest.EnrolledAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UpdateEnrollmentDto, Enrollment>();
        }
    }
}
