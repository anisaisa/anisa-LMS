using anisa_lms.DTOs;
using anisa_lms.Models;
using AutoMapper;

namespace anisa_lms.Mappings
{
    public class ModuleProfile : Profile
    {
        public ModuleProfile()
        {
            CreateMap<Module, ModuleDto>();

            CreateMap<CreateModuleDto, Module>();

            CreateMap<UpdateModuleDto, Module>();
        }
    }
}
