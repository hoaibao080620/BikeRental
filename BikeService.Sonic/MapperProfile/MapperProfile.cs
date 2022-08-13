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
        CreateMap<BikeUpdateDto, Bike>().ForMember(dest => dest.CreatedOn, src => src.Ignore());
        CreateMap<BikeStationInsertDto, BikeStation>().ReverseMap();
        CreateMap<BikeStationUpdateDto, BikeStation>().ForMember(dest => dest.CreatedOn, src => src.Ignore());
        CreateMap<BikeStation, BikeStationRetrieveDto>().ReverseMap();
    }
}
