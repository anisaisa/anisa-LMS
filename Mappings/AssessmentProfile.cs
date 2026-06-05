using anisa_lms.DTOs;
using anisa_lms.Models;
using AutoMapper;

namespace anisa_lms.Mappings
{
    public class AssessmentProfile : Profile
    {
        public AssessmentProfile()
        {
            CreateMap<Assessment, AssessmentDto>();

            CreateMap<CreateAssessmentDto, Assessment>();

            CreateMap<UpdateAssessmentDto, Assessment>();
        }
    }
}
