﻿using AutoMapper;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Dtos;
using BikeService.Sonic.Exceptions;
using BikeService.Sonic.Models;
using BikeService.Sonic.Services.Interfaces;
using Geolocation;

namespace BikeService.Sonic.BusinessLogics;

public class BikeStationBusinessLogic : IBikeStationBusinessLogic
{
    private readonly IGoogleMapService _googleMapService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public BikeStationBusinessLogic(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IGoogleMapService googleMapService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _googleMapService = googleMapService;
    }
    
    public async Task<BikeStationRetrieveDto> GetStationBike(int id)
    {
        var stationBike = await _unitOfWork.BikeStationRepository.GetById(id);
        return _mapper.Map<BikeStationRetrieveDto>(stationBike);
    }

    public async Task<List<BikeStationRetrieveDto>> GetAllStationBikes()
    {
        var stationBikes = await _unitOfWork.BikeStationRepository.All();
        return _mapper.Map<List<BikeStationRetrieveDto>>(stationBikes);
    }

    public async Task AddStationBike(BikeStationInsertDto bikeInsertDto)
    {
        var bikeStation = _mapper.Map<BikeStation>(bikeInsertDto);
        bikeStation.CreatedOn = DateTime.UtcNow;
        bikeStation.IsActive = true;
        var (latitude, longitude) = await _googleMapService.GetLocationOfAddress(bikeInsertDto.PlaceId);
        bikeStation.Latitude = latitude;
        bikeStation.Longitude = longitude;
        
        await _unitOfWork.BikeStationRepository.Add(bikeStation);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteStationBike(int id)
    {
        var bikeStation = await _unitOfWork.BikeStationRepository.GetById(id);
        if (bikeStation is null) throw new InvalidOperationException();
        
        await _unitOfWork.BikeStationRepository.Delete(bikeStation);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateStationBike(BikeStationUpdateDto bikeInsertDto)
    {
        await _unitOfWork.BikeStationRepository.Update(_mapper.Map<BikeStation>(bikeInsertDto));
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<BikeStationRetrieveDto> GetNearestBikeStationFromLocation(double longitude, double latitude)
    {
        var bikeLocations = await GetAllStationBikes();

        if (!bikeLocations.Any()) throw new NoBikeStationFoundException();
        
        var nearestBikeLocation = bikeLocations
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
