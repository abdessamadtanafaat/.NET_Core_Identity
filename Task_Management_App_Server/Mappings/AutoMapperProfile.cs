using AutoMapper;
using Task_Management_App.DTO;
using Task_Management_App.Models;

namespace Task_Management_App.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<UserDto, User>()
            .ForMember(dest => dest.PasswordHash, opt=>opt.MapFrom(src=> src.Password));
    }
}