using System.Linq.Expressions;
using System.Text.Json;
using AutoMapper;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.MessageType;
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
using Microsoft.Extensions.Caching.Distributed;
using BikeStationColor = BikeRental.MessageQueue.Events.BikeStationColor;

namespace BikeService.Sonic.BusinessLogics;

public class BikeStationBusinessLogic : IBikeStationBusinessLogic
{
    private readonly IGoogleMapService _googleMapService;
    private readonly IMessageQueuePublisher _messageQueuePublisher;
    private readonly IDistributedCache _distributedCache;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public BikeStationBusinessLogic(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IGoogleMapService googleMapService,
        IMessageQueuePublisher messageQueuePublisher,
        IDistributedCache distributedCache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _googleMapService = googleMapService;
        _messageQueuePublisher = messageQueuePublisher;
        _distributedCache = distributedCache;
    }
    
    public async Task<BikeStationRetrieveDto> GetStationBike(int id)
    {
        var stationBike = await _unitOfWork.BikeStationRepository.GetById(id);
        return _mapper.Map<BikeStationRetrieveDto>(stationBike);
    }

    public async Task<List<BikeStationRetrieveDto>> GetAllStationBikes(string email)
    {
        var isSuperManager =
            await _unitOfWork.ManagerRepository.Exists(x => x.Email == email && x.IsSuperManager);
        var stationBikes = await _unitOfWork.BikeStationRepository.Find(x => 
            isSuperManager || x.BikeStationManagers.Any(xx => xx.Manager.Email == email));
        var bikeStations = stationBikes.Select(x => new BikeStationRetrieveDto
        {
            Description = x.Description,
            Color = x.BikeStationColors.Any() ? x.BikeStationColors.FirstOrDefault()!.Color : null,
            Id = x.Id,
            Address = x.Address,
            Latitude = x.Latitude,
            Longitude = x.Longitude,
            Name = x.Name,
            ParkingSpace = x.ParkingSpace,
            UsedParkingSpace = x.Bikes.Count,
            ManagerIds = x.BikeStationManagers.Select(xx => xx.ManagerId).ToList(),
            CreatedOn = x.CreatedOn,
            UpdatedOn = x.UpdatedOn
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

        var bikeStationCode = bikeStation.Id.ToString().PadLeft(6);
        bikeStation.Code = bikeStationCode;
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
        var bikeStation = _mapper.Map<BikeStation>(bikeInsertDto);
        bikeStation.UpdatedOn = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(bikeInsertDto.PlaceId))
        {
            var (latitude, longitude) = await _googleMapService.GetLocationOfAddress(bikeInsertDto.PlaceId);
            bikeStation.Latitude = latitude;
            bikeStation.Longitude = longitude;
        }

        if (bikeInsertDto.ManagerIds.Any())
        {
            foreach (var managerId in bikeInsertDto.ManagerIds)
            {
                var isAlreadySave = await _unitOfWork.BikeStationManagerRepository
                    .Exists(x => x.ManagerId == managerId && x.BikeStationId == bikeStation.Id);

                if (!isAlreadySave)
                {
                    await _unitOfWork.BikeStationManagerRepository.Add(new BikeStationManager
                    {
                        ManagerId = managerId,
                        BikeStationId = bikeStation.Id,
                        IsActive = true,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow
                    });
                }
            }
        }
        
        await _unitOfWork.BikeStationRepository.Update(bikeStation);
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
                await _unitOfWork.BikeStationColorRepository.Add(new Models.BikeStationColor
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
        await _messageQueuePublisher.PublishBikeStationColorUpdatedEvent(new BikeStationColorUpdated
        {
            BikeStationColors = bikeStationColors.Select(x => new BikeStationColor
            {
                BikeStationId = x.BikeStationId,
                Color = x.Color
            }).ToList(),
            ManagerEmails = new List<string>
            {
                email
            },
            MessageType = MessageType.BikeStationColorUpdated
        });
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
            LicensePlate = x.BikeCode,
            Description = x.Description,
            UpdatedOn = x.UpdatedOn,
            Status = x.Status
        }).ToList();
    }

    public async Task<List<BikeStationNearMeDto>> GetBikeStationsNearMe(
        BikeStationRetrieveParameter bikeStationRetrieveParameter, 
        string email)
    {
        var key = string.Format(RedisCacheKey.BikeStationNearMeCache, email);
        var cache = await _distributedCache.GetStringAsync(key);

        if (!string.IsNullOrEmpty(cache))
        {
            return JsonSerializer.Deserialize<List<BikeStationNearMeDto>>(cache) ?? new List<BikeStationNearMeDto>();
        }
        
        var bikeStations = (await _unitOfWork.BikeStationRepository.All()).Select(x => new BikeStationNearMeDto
        {
            Description = x.Description,
            Id = x.Id,
            Address = x.Address,
            Latitude = x.Latitude,
            Longitude = x.Longitude,
            Name = x.Name,
            UsedParkingSpace = x.Bikes.Count,
            ParkingSpace = x.ParkingSpace
        }).Take(bikeStationRetrieveParameter.Limit).ToList();
        
        var originLocation = new GoogleMapLocation
        {
            Longitude = bikeStationRetrieveParameter.Longitude,
            Latitude = bikeStationRetrieveParameter.Latitude
        };
        
        foreach (var bikeStation in bikeStations)
        {
            try
            {
                var destination = new GoogleMapLocation
                {
                    Latitude = bikeStation.Latitude,
                    Longitude = bikeStation.Longitude
                };

                bikeStation.Distance =
                    await _googleMapService.GetDistanceBetweenTwoLocations(originLocation, destination);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(bikeStation.Id);
            }
        }
        
        var bikeStationsNearMe = bikeStations.OrderBy(x => x.Distance).ToList();
        await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(bikeStationsNearMe), new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(1)
        });
        return bikeStationsNearMe;
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
                x.BikeStation.BikeStationColors.First().Color : null,
            BikeStationCode = x.BikeStation.Code
        }).ToList();
        
        foreach (var bikeUpdated in bikesUpdated)
        {
            await _messageQueuePublisher.PublishBikeUpdatedEvent(new BikeUpdated
            {
                Id = bikeUpdated.Id,
                Color = bikeUpdated.Color,
                BikeStationId = bikeUpdated.BikeStationId,
                BikeStationName = bikeUpdated.BikeStationName,
                BikeStationCode = bikeUpdated.BikeStationCode,
                MessageType = MessageType.BikeUpdated
            }); 
        }
    }

    public async Task<List<BikeStationAssignDto>> GetAssignableBikeStations(int totalBikeAssign)
    {
        var bikeStations = (await _unitOfWork.BikeStationRepository
                .Find(x => (x.ParkingSpace - x.Bikes.Count) >= totalBikeAssign))
            .AsNoTracking()
            .Select(x => new BikeStationAssignDto
            {
                Id = x.Id,
                Name = x.Name,
                ParkingSpace = x.ParkingSpace,
                UsedParkingSpace = x.Bikes.Count
            });

        return bikeStations.ToList();
    }

    public async Task<List<AssignableManager>> GetAssignableManagers()
    {
        var managers = (await _unitOfWork.ManagerRepository.Find(x => x.IsSuperManager == false))
            .AsNoTracking()
            .Select(x => new AssignableManager
            {
                Id = x.Id,
                Name = x.Email
            }).ToList();

        return managers;
    }

    public async Task<List<AssignableStation>> GetAssignableStations()
    {
        var stations = (await _unitOfWork.BikeStationRepository.Find(x => x.BikeStationManagers.Count < 3))
            .AsNoTracking()
            .Select(x => new AssignableStation
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

        return stations;
    }

    public async Task AssignBikeStationsToManager(BikeStationManagerAssignDto bikeStationManagerAssign)
    {
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
