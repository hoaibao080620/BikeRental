using System.Linq.Expressions;
using AutoMapper;
using BikeService.Sonic.Const;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Dtos;
using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Dtos.BikeStation;
using BikeService.Sonic.Models;
using BikeService.Sonic.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BikeService.Sonic.BusinessLogics;

public class BikeStationBusinessLogic : IBikeStationBusinessLogic
{
    private readonly IGoogleMapService _googleMapService;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public BikeStationBusinessLogic(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IGoogleMapService googleMapService,
        ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _googleMapService = googleMapService;
        _cacheService = cacheService;
    }
    
    public async Task<BikeStationRetrieveDto> GetStationBike(int id)
    {
        var stationBike = await _unitOfWork.BikeStationRepository.GetById(id);
        return _mapper.Map<BikeStationRetrieveDto>(stationBike);
    }

    public async Task<List<BikeStationRetrieveDto>> GetAllStationBikes()
    {
        var stationBikes = await _unitOfWork.BikeStationRepository.All();
        return stationBikes.Select(x => new BikeStationRetrieveDto
        {
            Description = x.Description,
            Color = x.BikeStationColors.Any() ? x.BikeStationColors.FirstOrDefault()!.Color : null,
            Id = x.Id,
            Address = x.Address,
            Latitude = x.Latitude,
            Longitude = x.Longitude,
            Name = x.Name,
            UsedParkingSpace = x.UsedParkingSpace
        }).ToList();
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
        // var bikeLocations = await GetAllStationBikes();
        //
        // if (!bikeLocations.Any()) throw new NoBikeStationFoundException();
        //
        // var nearestBikeLocation = bikeLocations
        //     .Select(x => new
        // {
        //     Distance = GeoCalculator.GetDistance(
        //         latitude, longitude,
        //         x.Latitude,
        //         x.Longitude),
        //     BikeStation = x
        // }).OrderBy(x => x.Distance).Select(x => x.BikeStation).FirstOrDefault();
        //
        // return nearestBikeLocation!;

        return new BikeStationRetrieveDto();
    }

    public async Task UpdateBikeStationColor(List<BikeStationColorDto> bikeStationColors, string email)
    {
        var bikeIds = (await _unitOfWork.BikeRepository
            .Find(x => x.BikeStationId.HasValue && bikeStationColors.Select(b => b.BikeStationId)
                .Contains(x.BikeStationId.Value))).ToList();
        
        var manager = (await _unitOfWork.ManagerRepository.Find(x => x.Email == email)).FirstOrDefault();
        foreach (var bikeStationColorDto in bikeStationColors)
        {
            var bikeStationColor = (await _unitOfWork.BikeStationColorRepository.Find(x =>
                x.Manager.Email == email && x.BikeStationId == bikeStationColorDto.BikeStationId)).FirstOrDefault();
        
            if (bikeStationColor is null)
            {
                await _unitOfWork.BikeStationColorRepository.Add(new BikeStationColor
                {
                    BikeStationId = bikeStationColorDto.BikeStationId,
                    Manager = manager!,
                    Color = bikeStationColorDto.Color,
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true
                });
            }
            else
            {
                bikeStationColor.Color = bikeStationColorDto.Color;
                bikeStationColor.UpdatedOn = DateTime.UtcNow;
            }
        }

        await _unitOfWork.SaveChangesAsync();

        foreach (var bikeId in bikeIds)
        {
            await _cacheService.Remove(string.Format(RedisCacheKey.SingleBike, bikeId));
        }
    }

    public async Task<List<BikeStationColorRetrieveDto>> GetBikeStationColors(string email)
    {
        var manager = (await _unitOfWork.ManagerRepository.Find(x => x.Email == email)).FirstOrDefault();
        
        ArgumentNullException.ThrowIfNull(manager);
        Expression<Func<BikeStation, bool>> expressionFilter = manager.IsSuperManager ?
            _ => true :
            x => x.BikeStationManagers.Any(bs => bs.ManagerId == manager.Id); 
        
        return (await _unitOfWork.BikeStationRepository.Find(expressionFilter)).Select(x => new BikeStationColorRetrieveDto
        {
            BikeStationId = x.Id,
            BikeStationName = x.Name,
            Color = x.BikeStationColors.Any(bsc => bsc.BikeStationId == x.Id && bsc.Manager.Email == email) ?
                x.BikeStationColors.FirstOrDefault(bsc => bsc.BikeStationId == x.Id && bsc.Manager.Email == email)!.Color :
                null
        }).ToList();
    }

    public async Task<List<BikeRetrieveDto>> GetBikeStationBike(int bikeStationId)
    {
        var bikes = await _unitOfWork.BikeRepository.Find(x => x.BikeStationId == bikeStationId);

        return bikes.AsNoTracking().Select(x => new BikeRetrieveDto
        {
            Id = x.Id,
            BikeStationId = x.BikeStationId,
            BikeStationName = x.BikeStation!.Name,
            LicensePlate = x.LicensePlate,
            Description = x.Description,
            IsActive = x.IsActive,
            CreatedOn = x.CreatedOn,
            UpdatedOn = x.UpdatedOn,
            Status = x.Status
        }).ToList();
    }
}
