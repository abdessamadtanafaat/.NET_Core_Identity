using AutoMapper;
using Authentication_Authorisation.DTO;
using Authentication_Authorisation.Models;

namespace Authentication_Authorisation.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<RegisterDto, User>()
            .ForMember(dest => dest.PasswordHash, opt=>opt.MapFrom(src=> src.Password));
    }
}