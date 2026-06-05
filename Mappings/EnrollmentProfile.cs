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
                .ForMember(
                    dest => dest.StudentFullName,
                    opt => opt.MapFrom(src => src.Student != null ? src.Student.FullName : ""));

            CreateMap<CreateEnrollmentDto, Enrollment>()
                .ForMember(dest => dest.EnrolledAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UpdateEnrollmentDto, Enrollment>();
        }
    }
}
