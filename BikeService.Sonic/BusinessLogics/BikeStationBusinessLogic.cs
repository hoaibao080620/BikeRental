using System.Linq.Expressions;
using AutoMapper;
using BikeRental.MessageQueue.Events;
using BikeService.Sonic.Const;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Dtos;
using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Dtos.BikeStation;
using BikeService.Sonic.Dtos.GoogleMapAPI;
using BikeService.Sonic.MessageQueue.Publisher;
using BikeService.Sonic.Models;
using BikeService.Sonic.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BikeService.Sonic.BusinessLogics;

public class BikeStationBusinessLogic : IBikeStationBusinessLogic
{
    private readonly IGoogleMapService _googleMapService;
    private readonly IMessageQueuePublisher _messageQueuePublisher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public BikeStationBusinessLogic(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IGoogleMapService googleMapService,
        IMessageQueuePublisher messageQueuePublisher)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _googleMapService = googleMapService;
        _messageQueuePublisher = messageQueuePublisher;
    }
    
    public async Task<BikeStationRetrieveDto> GetStationBike(int id)
    {
        var stationBike = await _unitOfWork.BikeStationRepository.GetById(id);
        return _mapper.Map<BikeStationRetrieveDto>(stationBike);
    }

    public async Task<List<BikeStationRetrieveDto>> GetAllStationBikes()
    {
        var stationBikes = await _unitOfWork.BikeStationRepository.All();
        var bikeStations = stationBikes.Select(x => new BikeStationRetrieveDto
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

        return bikeStations;
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
        
        var bikesColor = (await _unitOfWork.BikeRepository
            .Find(x => x.BikeStationId.HasValue && bikeStationColors.Select(b => b.BikeStationId)
                .Contains(x.BikeStationId.Value))).Select(x => new
        {
            BikeId = x.Id,
            Color = x.BikeStation!.BikeStationColors.Any(b => b.BikeStationId == x.BikeStationId) ?
                x.BikeStation!.BikeStationColors.First().Color : null
        }).ToList();
        
        foreach (var bikeColor in bikesColor)
        {
            await _messageQueuePublisher.PublishBikeUpdatedEvent(new BikeUpdated
            {
                Id = bikeColor.BikeId,
                Color = bikeColor.Color
            }); 
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
            UpdatedOn = x.UpdatedOn,
            Status = x.Status
        }).ToList();
    }

    public async Task<List<BikeStationRetrieveDto>> GetBikeStationsNearMe(BikeStationRetrieveParameter bikeStationRetrieveParameter)
    {
        var bikeStations = (await GetAllStationBikes()).Take(bikeStationRetrieveParameter.Limit).ToList();
        var originLocation = new GoogleMapLocation
        {
            Longitude = bikeStationRetrieveParameter.Longitude,
            Latitude = bikeStationRetrieveParameter.Latitude
        };
        
        foreach (var bikeStation in bikeStations)
        {
            var destination = new GoogleMapLocation
            {
                Latitude = bikeStation.Latitude,
                Longitude = bikeStation.Longitude
            };

            bikeStation.Distance = await _googleMapService.GetDistanceBetweenTwoLocations(originLocation, destination);
        }

        return bikeStations;
    }

    public async Task AssignBikesToBikeStation(BikeStationBikeAssignDto bikeAssignDto)
    {
        var bikes = await _unitOfWork.BikeRepository.Find(x => bikeAssignDto.BikeIds.Contains(x.Id));

        foreach (var bike in bikes.ToList())
        {
            bike.BikeStationId = bikeAssignDto.BikeStationId;
        }

        await _unitOfWork.SaveChangesAsync();

        var bikesUpdated = bikes.Select(x => new
        {
            x.Id,
            x.BikeStationId,
            BikeStationName = x.BikeStation!.Name,
            Color = x.BikeStation.BikeStationColors.Any() ?
                x.BikeStation.BikeStationColors.First().Color : null
        }).ToList();
        
        foreach (var bikeUpdated in bikesUpdated)
        {
            await _messageQueuePublisher.PublishBikeUpdatedEvent(new BikeUpdated
            {
                Id = bikeUpdated.Id,
                Color = bikeUpdated.Color,
                BikeStationId = bikeUpdated.BikeStationId,
                BikeStationName = bikeUpdated.BikeStationName
            }); 
        }
    }

    public async Task<List<BikeStationAssignDto>> GetAssignableBikeStations(int totalBikeAssign)
    {
        var bikeStations = (await _unitOfWork.BikeStationRepository
                .Find(x => (x.ParkingSpace - x.UsedParkingSpace) >= totalBikeAssign))
            .AsNoTracking()
            .Select(x => new BikeStationAssignDto
            {
                Id = x.Id,
                Name = x.Name,
                ParkingSpace = x.ParkingSpace,
                UsedParkingSpace = x.UsedParkingSpace
            });

        return bikeStations.ToList();
    }

    public async Task<List<AssignableManager>> GetAssignableManagers()
    {
        var managers = (await _unitOfWork.ManagerRepository.All())
            .AsNoTracking()
            .Select(x => new AssignableManager
            {
                Id = x.Id,
                Name = x.Email
            }).ToList();

        return managers;
    }

    public async Task AssignBikeStationsToManager(BikeStationManagerAssignDto bikeStationManagerAssign)
    {
        var bikeStations = await _unitOfWork.BikeStationManagerRepository
            .Find(x => bikeStationManagerAssign.BikeStationIds.Contains(x.BikeStationId));
        foreach (var bikeStationId in bikeStationManagerAssign.BikeStationIds)
        {
            var bikeStationManager = (await _unitOfWork.BikeStationManagerRepository
                .Find(x => bikeStationManagerAssign.BikeStationIds.Contains(x.BikeStationId))).FirstOrDefault();

            if (bikeStationManager is null)
            {
                await _unitOfWork.BikeStationManagerRepository.Add(new BikeStationManager
                {
                    BikeStationId = bikeStationId,
                    ManagerId = bikeStationManagerAssign.ManagerId,
                    CreatedOn = DateTime.UtcNow,
                    IsActive = true,
                    UpdatedOn = DateTime.UtcNow
                });
            }
            else
            {
                bikeStationManager.ManagerId = bikeStationManagerAssign.ManagerId;
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }
}
