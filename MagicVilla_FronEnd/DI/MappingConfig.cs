using AutoMapper;
using MagicVilla_FronEnd.Models.DTOS;

namespace MagicVilla_FronEnd.DI
{
    public class MappingConfig : Profile
    {
        public MappingConfig() 
        {
            CreateMap<VillaDto, VillaCreateDto>().ReverseMap();
            CreateMap<VillaDto, VillaUpdateDto>().ReverseMap();

            CreateMap<VillaNumberDto, VillaNumberCreateDto>().ReverseMap();
            CreateMap<VillaNumberDto, VillaNumberUpdateDto>().ReverseMap();
        }
    }
}
