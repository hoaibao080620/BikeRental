using AutoMapper;
using UserService.Dtos;
using UserService.Models;

namespace UserService.Profiles;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<UserInsertDto, User>();
        CreateMap<UserUpdateDto, User>();
        CreateMap<User, UserRetrieveDto>();
    }
}
