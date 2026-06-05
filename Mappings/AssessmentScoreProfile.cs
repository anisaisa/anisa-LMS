using anisa_lms.DTOs;
using anisa_lms.Models;
using AutoMapper;

namespace anisa_lms.Mappings
{
    public class AssessmentScoreProfile : Profile
    {
        public AssessmentScoreProfile()
        {
            CreateMap<AssessmentScore, AssessmentScoreDto>()
                .ForMember(
                    dest => dest.StudentFullName,
                    opt => opt.MapFrom(src => src.Student != null ? src.Student.FullName : ""));

            CreateMap<CreateAssessmentScoreDto, AssessmentScore>();

            CreateMap<UpdateAssessmentScoreDto, AssessmentScore>();
        }
    }
}
