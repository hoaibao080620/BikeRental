using AutoMapper;
using BikeService.Sonic.Dtos;
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
        CreateMap<BikeStation, BikeStationInsertDto>().ReverseMap();
        CreateMap<BikeStation, BikeStationUpdateDto>().ReverseMap();
        CreateMap<BikeStation, BikeStationRetrieveDto>().ReverseMap();
    }
}