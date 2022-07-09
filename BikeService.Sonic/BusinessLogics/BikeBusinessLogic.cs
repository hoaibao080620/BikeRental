using AutoMapper;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.MessageType;
using BikeService.Sonic.Const;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Exceptions;
using BikeService.Sonic.MessageQueue.Publisher;
using BikeService.Sonic.Models;
using BikeService.Sonic.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BikeService.Sonic.BusinessLogics;

public class BikeBusinessLogic : IBikeBusinessLogic
{
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageQueuePublisher _messageQueuePublisher;

    public BikeBusinessLogic(
        IMapper mapper,
        ICacheService cacheService,
        IUnitOfWork unitOfWork,
        IMessageQueuePublisher messageQueuePublisher)
    {
        _mapper = mapper;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
        _messageQueuePublisher = messageQueuePublisher;
    }

    public async Task<BikeRetrieveDto?> GetBike(int id)
    {
        var bike = await (await _unitOfWork.BikeRepository.Find(x => x.Id == id))
            .AsNoTracking().Select(b => new BikeRetrieveDto
            {
                BikeStationId = b.BikeStationId,
                BikeStationName = b.BikeStation != null ? b.BikeStation.Name : null,
                Id = b.Id,
                Description = b.Description,
                LicensePlate = b.LicensePlate,
                Status = b.Status,
                UpdatedOn = b.UpdatedOn
            }).FirstOrDefaultAsync() ?? throw new BikeNotFoundException(id);
        
        return bike;
    }

    public async Task<List<BikeRetrieveDto>> GetBikes(string managerEmail)
    {
        var bikes = (await _unitOfWork.BikeRepository
                .Find(x => x.BikeStation != null && x.BikeStation.BikeStationManagers
                    .Any(b => b.Manager.Email == managerEmail)))
                .AsNoTracking().Select(b => new BikeRetrieveDto
                {
                    BikeStationId = b.BikeStationId,
                    BikeStationName = b.BikeStation != null ? b.BikeStation.Name : null,
                    Id = b.Id,
                    Description = b.Description,
                    LicensePlate = b.LicensePlate,
                    Status = b.Status,
                    UpdatedOn = b.UpdatedOn
                }).ToList();
        
        return bikes;
    }

    public async Task AddBike(BikeInsertDto bikeInsertDto)
    {
        var bikeStationColor = (await _unitOfWork.BikeStationColorRepository
            .Find(x => x.BikeStationId == bikeInsertDto.BikeStationId)).FirstOrDefault();
            
        var bike = _mapper.Map<Bike>(bikeInsertDto);
        bike.Status = BikeStatus.Available;
        await _unitOfWork.BikeRepository.Add(bike);
        await _unitOfWork.SaveChangesAsync();
        var bikeStation = bike.BikeStationId.HasValue ?
            await _unitOfWork.BikeStationRepository.GetById(bike.BikeStationId.Value) : null;
        await _messageQueuePublisher.PublishBikeCreatedEvent(new BikeCreated
        {
            Id = bike.Id,
            BikeStationId = bike.BikeStationId,
            BikeStationName = bikeStation?.Name,
            Description = bike.Description,
            LicensePlate = bike.LicensePlate,
            Status = bike.Status,
            MessageType = MessageType.BikeCreated,
            Color = bikeStationColor?.Color
        });
    }

    public async Task UpdateBike(BikeUpdateDto bikeInsertDto)
    {
        var bike = _mapper.Map<Bike>(bikeInsertDto);
        bike.UpdatedOn = DateTime.UtcNow;
        var bikeStation = bike.BikeStationId.HasValue ?
            await _unitOfWork.BikeStationRepository.GetById(bike.BikeStationId.Value) : null;
        await _cacheService.Remove(string.Format(RedisCacheKey.SingleBike, bike.Id));
        await _unitOfWork.BikeRepository.Update(bike);
        await _unitOfWork.SaveChangesAsync();
        await _messageQueuePublisher.PublishBikeUpdatedEvent(new BikeUpdated
        {
            Id = bike.Id,
            BikeStationId = bike.BikeStationId,
            BikeStationName = bikeStation?.Name,
            Description = bike.Description,
            LicensePlate = bike.LicensePlate,
            MessageType = MessageType.BikeUpdated
        });
    }

    public async Task DeleteBike(int id)
    {
        var bike = await _unitOfWork.BikeRepository.GetById(id);
        if (bike is null) throw new InvalidOperationException();
        
        await _unitOfWork.BikeRepository.Delete(bike);
        await _unitOfWork.SaveChangesAsync();
        await _messageQueuePublisher.PublishBikeDeletedEvent(id);
    }

    // public async Task<BikeRentingStatus> GetBikeRentingStatus(string accountEmail)
    // {
    //     var rentingStatus = await _unitOfWork.BikeRentalTrackingRepository
    //         .Find(b => !b.CheckoutOn.HasValue);
    //
    //     return rentingStatus.Any()
    //         ? rentingStatus.Select(x => new BikeRentingStatus
    //         {
    //             AccountEmail = accountEmail,
    //             IsRenting = true,
    //             BikeId = x.BikeId,
    //             LicensePlate = x.Bike.LicensePlate,
    //             LastLatitude = x.Bike.BikeLocationTrackings.FirstOrDefault(b => b.IsActive)!.Latitude,
    //             LastLongitude = x.Bike.BikeLocationTrackings.FirstOrDefault(b => b.IsActive)!.Longitude,
    //             LastAddress = x.Bike.BikeLocationTrackings.FirstOrDefault(b => b.IsActive)!.Address,
    //         }).FirstOrDefault()!
    //         : new BikeRentingStatus
    //         {
    //             AccountEmail = accountEmail,
    //             IsRenting = false,
    //             BikeId = null
    //         };
    // }

    public async Task DeleteBikes(List<int> bikeIds)
    {
        var bikes = (await _unitOfWork.BikeRepository.Find(x => bikeIds.Contains(x.Id))).ToList();
        if (bikes.Any(x => x.Status == BikeStatus.InUsed || x.BikeStationId.HasValue))
            throw new InvalidOperationException("Cannot delete bike with status in used or belong to a bike station");
        
        foreach (var bike in bikes)
        {
            await _cacheService.Remove(string.Format(RedisCacheKey.SingleBike, bike.Id));
            await _unitOfWork.BikeRepository.Delete(bike);
            await _unitOfWork.SaveChangesAsync();
            await _messageQueuePublisher.PublishBikeDeletedEvent(bike.Id);
        }
    }
    
    private async Task<Bike> GetBikeById(int bikeId)
    {
        var bike = await _unitOfWork.BikeRepository.GetById(bikeId) ?? throw new BikeNotFoundException(bikeId);
        return bike ?? throw new BikeNotFoundException(bikeId);
    }

    // private async Task UpdateBikeCache(BikeCacheParameter bikeCacheParameter)
    // {
    //     var key = string.Format(RedisCacheKey.SingleBike, bikeCacheParameter.BikeId);
    //     var bikeCache = await _cacheService.Get(key);
    //
    //     if (bikeCache is null) return;
    //     var bike = JsonSerializer.Deserialize<BikeRetrieveDto>(bikeCache);
    //     ArgumentNullException.ThrowIfNull(bike);
    //     bike.Status = bikeCacheParameter.Status ?? bike.Status;
    //     await _cacheService.Add(key, JsonSerializer.Serialize(bike));
    // }
    //
    // private static double GetRentingPoint(DateTime checkinOn, DateTime checkoutOn)
    // {
    //     var duration = checkoutOn.Subtract(checkinOn).TotalHours;
    //
    //     return duration switch
    //     {
    //         <= TimeDuration.TotalHourOfDay => duration * 2,
    //         <= TimeDuration.TotalHourOfWeek => duration * 1.0 / TimeDuration.TotalHourOfDay * 20,
    //         _ => duration * 1.0 / TimeDuration.TotalHourOfWeek * 100
    //     };
    // }
}
