using anisa_lms.DTOs;
using anisa_lms.Models;
using AutoMapper;

namespace anisa_lms.Mappings
{
    public class StudentModuleProgressProfile : Profile
    {
        public StudentModuleProgressProfile()
        {
            CreateMap<StudentModuleProgress, StudentModuleProgressDto>()
                .ForMember(dest => dest.ModuleId, opt => opt.MapFrom(src => src.ModuleId))
                .ForMember(
                    dest => dest.StudentFullName,
                    opt => opt.MapFrom(src => src.Student != null ? src.Student.FullName : ""));

            CreateMap<CreateStudentModuleProgressDto, StudentModuleProgress>();

            CreateMap<UpdateStudentModuleProgress, StudentModuleProgress>();
        }
    }
}
