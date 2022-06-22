using AutoMapper;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Dtos;
using BikeService.Sonic.Exceptions;
using BikeService.Sonic.Models;
using BikeService.Sonic.Services.Interfaces;
using Geolocation;

namespace BikeService.Sonic.BusinessLogics;

public class BikeStationBusinessLogic : IBikeStationBusinessLogic
{
    private readonly IBikeLocationHub _bikeLocationHub;
    private readonly IBikeStationManagerRepository _bikeStationManagerRepository;
    private readonly IBikeStationRepository _bikeStationRepository;
    private readonly IMapper _mapper;
    public BikeStationBusinessLogic(IBikeLocationHub bikeLocationHub, 
        IBikeStationManagerRepository bikeStationManagerRepository,
        IBikeRepository bikeRepository,
        IMapper mapper, 
        IBikeStationRepository bikeStationRepository)
    {
        _bikeLocationHub = bikeLocationHub;
        _bikeStationManagerRepository = bikeStationManagerRepository;
        
        _mapper = mapper;
        _bikeStationRepository = bikeStationRepository;
    }


    public async Task<BikeStationRetrieveDto> GetStationBike(int id)
    {
    
        var stationBike =await _bikeStationRepository.GetById(id);
        return _mapper.Map<BikeStationRetrieveDto>(stationBike);
    }

    public async Task<List<BikeStationRetrieveDto>> GetAllStationBikes()
    {
        var stationBikes = await _bikeStationRepository.All();
        return _mapper.Map<List<BikeStationRetrieveDto>>(stationBikes);
    }

    public async Task AddStationBike(BikeStationInsertDto bikeInsertDto)
    {
        await _bikeStationRepository.Add(_mapper.Map<BikeStation>(bikeInsertDto));
        await _bikeStationRepository.SaveChanges();
    }

    public async Task DeleteStationBike(int id)
    {
        var bikeStation = await _bikeStationRepository.GetById(id);
        if (bikeStation is null) throw new InvalidOperationException();
        
        await _bikeStationRepository.Delete(bikeStation);
        await _bikeStationRepository.SaveChanges();
    }

    public async Task UpdateStationBike(BikeStationUpdateDto bikeInsertDto)
    {
        await _bikeStationRepository.Update(_mapper.Map<BikeStation>(bikeInsertDto));
        await _bikeStationRepository.SaveChanges();
    }

    public async Task<BikeStationRetrieveDto> GetNearestBikeStationFromLocation(double longitude, double latitude)
    {
        var bikeLocations = await GetAllStationBikes();

        if (!bikeLocations.Any()) throw new NoBikeStationFoundException();
        
        var nearestBikeLocation = bikeLocations.Where(x => x.ParkingSpace - x.UsedParkingSpace != 0)
            .Select(x => new
        {
            Distance = GeoCalculator.GetDistance(
                latitude, longitude,
                x.Latitude,
                x.Longitude),
            BikeStation = x
        }).OrderBy(x => x.Distance).Select(x => x.BikeStation).FirstOrDefault();

        return nearestBikeLocation!;
    }
}
