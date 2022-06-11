using AutoMapper;
using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Models;

namespace BikeService.Sonic.MapperProfile;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Bike, BikeRetrieveDto>().ReverseMap();
        CreateMap<Bike, BikeInsertDto>().ReverseMap();
        CreateMap<Bike, BikeUpdateDto>().ReverseMap();
    }
}