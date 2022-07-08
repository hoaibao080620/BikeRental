using System.Text.Json;
using AutoMapper;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.MessageType;
using BikeService.Sonic.Const;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Dtos.BikeOperation;
using BikeService.Sonic.Dtos.History;
using BikeService.Sonic.Exceptions;
using BikeService.Sonic.MessageQueue.Publisher;
using BikeService.Sonic.Models;
using BikeService.Sonic.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BikeService.Sonic.BusinessLogics;

public class BikeBusinessLogic : IBikeBusinessLogic
{
    private readonly IMapper _mapper;
    private readonly IBikeLoaderAdapter _bikeLoaderAdapter;
    private readonly IGoogleMapService _googleMapService;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageQueuePublisher _messageQueuePublisher;

    public BikeBusinessLogic(
        IMapper mapper,
        IBikeLoaderAdapter bikeLoaderAdapter,
        IGoogleMapService googleMapService,
        ICacheService cacheService,
        IUnitOfWork unitOfWork,
        IMessageQueuePublisher messageQueuePublisher)
    {
        _mapper = mapper;
        _bikeLoaderAdapter = bikeLoaderAdapter;
        _googleMapService = googleMapService;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
        _messageQueuePublisher = messageQueuePublisher;
    }

    public async Task<BikeRetrieveDto?> GetBike(int id)
    {
        return await _bikeLoaderAdapter.GetBike(id);
    }

    public async Task<List<BikeRetrieveDto>> GetBikes(string managerEmail)
    {
        var bikeIds = await _bikeLoaderAdapter.GetBikeIdsOfManager(managerEmail);
        var bikes = new List<BikeRetrieveDto>();
        foreach (var bikeId in bikeIds)
        {
            var bike = await _bikeLoaderAdapter.GetBike(bikeId);
            bikes.Add(bike);
        }
        
        return bikes.ToList();
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

    public async Task BikeChecking(BikeCheckinDto bikeCheckinDto, string accountEmail)
    {
        var managerEmails = await _unitOfWork.BikeStationManagerRepository
            .GetManagerEmailsByBikeId(bikeCheckinDto.BikeId);

        var bike = await GetBikeById(bikeCheckinDto.BikeId);
        var address = await _googleMapService.GetAddressOfLocation(
            bikeCheckinDto.Longitude,
            bikeCheckinDto.Latitude);
        ArgumentNullException.ThrowIfNull(address);
        
        var bikeStation = await _unitOfWork.BikeStationRepository.GetById(bike.BikeStationId!.Value);
        bikeStation!.UsedParkingSpace++;
        
        var pushEventToMapTask = _messageQueuePublisher.PublishBikeLocationChangeCommand(managerEmails);
        var pushNotificationToManagers = _messageQueuePublisher.PublishBikeCheckedInEvent(
            new BikeCheckedIn
            {
                ManagerEmails = managerEmails,
                BikeId = bike.Id,
                BikeStationId = bikeStation.Id,
                BikeStationName = bikeStation.Name,
                AccountEmail = accountEmail,
                LicensePlate = bike.LicensePlate,
                CheckinOn = bikeCheckinDto.CheckinTime,
                MessageType = MessageType.BikeCheckedIn
            });
        
        var startTrackingBikeTask = StartTrackingBike(bikeCheckinDto, address, accountEmail);
        var updateCachedTask = UpdateBikeCache(new BikeCacheParameter
        {
            BikeId = bike.Id,
            Longitude = bikeCheckinDto.Longitude,
            Latitude = bikeCheckinDto.Latitude,
            IsRenting = true,
            Address = address,
            Status = BikeStatus.InUsed
        });
        
        bike.Status = BikeStatus.InUsed;
        await Task.WhenAll(
            pushEventToMapTask, 
            startTrackingBikeTask,
            updateCachedTask,
            pushNotificationToManagers
        );
        
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task BikeCheckout(BikeCheckoutDto bikeCheckout, string accountEmail)
    {
        var managerEmails = await _unitOfWork.BikeStationManagerRepository
            .GetManagerEmailsByBikeId(bikeCheckout.BikeId);
        
        var bike = await GetBikeById(bikeCheckout.BikeId);
        bike.Status = BikeStatus.Available;
        bike.BikeStationId = bikeCheckout.BikeStationId;

        var bikeStation = await _unitOfWork.BikeStationRepository.GetById(bikeCheckout.BikeStationId!.Value);

        var address = await _googleMapService.GetAddressOfLocation(
            bikeCheckout.Longitude,
            bikeCheckout.Latitude);

        bikeCheckout.Address = address;
        var pushEventToMapTask = _messageQueuePublisher.PublishBikeLocationChangeCommand(managerEmails);
        var stopTrackingBikeTask = StopTrackingBike(bikeCheckout, accountEmail);
        var updateBikeCache = UpdateBikeCache(new BikeCacheParameter
        {
            BikeId = bike.Id,
            Longitude = bikeCheckout.Longitude,
            Latitude = bikeCheckout.Latitude,
            Address = address,
            IsRenting = false,
            Status = BikeStatus.Available
        });
        
        var pushNotificationToManagers = _messageQueuePublisher.PublishBikeCheckedOutEvent(
            new BikeCheckedOut
            {
                ManagerEmails = managerEmails,
                BikeId = bike.Id,
                BikeStationId = bikeStation!.Id,
                BikeStationName = bikeStation.Name,
                AccountEmail = accountEmail,
                LicensePlate = bike.LicensePlate,
                CheckoutOn = bikeCheckout.CheckoutOn,
                MessageType = MessageType.BikeCheckedOut
            });
        
        await Task.WhenAll(
            pushEventToMapTask, 
            stopTrackingBikeTask, 
            updateBikeCache, 
            pushNotificationToManagers);
        
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateBikeLocation(BikeLocationDto bikeLocationDto)
    {
        var managerEmails = await _unitOfWork.BikeStationManagerRepository
            .GetManagerEmailsByBikeId(bikeLocationDto.BikeId);
        
        var bike = await GetBikeById(bikeLocationDto.BikeId);
        bikeLocationDto.Address = await _googleMapService.GetAddressOfLocation(
            bikeLocationDto.Longitude, 
            bikeLocationDto.Latitude);

        var updateBikeTracking = UpdateBikeRentalTracking(bikeLocationDto);
        
        var pushEventTask = _messageQueuePublisher.PublishBikeLocationChangeCommand(managerEmails);
        var updateBikeCache = UpdateBikeCache(new BikeCacheParameter
        {
            BikeId = bike.Id,
            Longitude = bikeLocationDto.Longitude,
            Latitude = bikeLocationDto.Latitude,
            Address = bikeLocationDto.Address
        });

        await Task.WhenAll(pushEventTask, updateBikeCache, updateBikeTracking);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<BikeRentingStatus> GetBikeRentingStatus(string accountEmail)
    {
        var rentingStatus = await _unitOfWork.BikeRentalTrackingRepository
            .Find(b => !b.CheckoutOn.HasValue);

        return rentingStatus.Any()
            ? rentingStatus.Select(x => new BikeRentingStatus
            {
                AccountEmail = accountEmail,
                IsRenting = true,
                BikeId = x.BikeId,
                LicensePlate = x.Bike.LicensePlate,
                LastLatitude = x.Bike.BikeLocationTrackings.FirstOrDefault(b => b.IsActive)!.Latitude,
                LastLongitude = x.Bike.BikeLocationTrackings.FirstOrDefault(b => b.IsActive)!.Longitude,
                LastAddress = x.Bike.BikeLocationTrackings.FirstOrDefault(b => b.IsActive)!.Address,
            }).FirstOrDefault()!
            : new BikeRentingStatus
            {
                AccountEmail = accountEmail,
                IsRenting = false,
                BikeId = null
            };
    }

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

    public async Task<List<BikeRentingHistory>> GetBikeRentingHistories(string email)
    {
        var manager = (await _unitOfWork.ManagerRepository.Find(x => x.Email == email)).FirstOrDefault()!;

        var bikeRentingHistories = (await _unitOfWork.BikeRentalTrackingRepository
            .Find(x =>
                manager.IsSuperManager ||
                x.Bike.BikeStation!.BikeStationManagers.Any(b => b.Manager.Email == email)
                && x.IsActive
            ))
            .AsNoTracking()
            .Select(x => new BikeRentingHistory
            {
                Id = x.Id,
                BikeId = x.BikeId,
                BikePlate = x.Bike.LicensePlate,
                AccountEmail = x.Account.Email,
                BikeStationName = x.Bike.BikeStation!.Name,
                CheckedInOn = x.CheckinOn,
                CheckedOutOn = x.CheckoutOn,
                TotalTime = x.CheckoutOn.HasValue ? x.CheckoutOn.Value.Subtract(x.CheckinOn).TotalMinutes : null
            });

        return bikeRentingHistories.ToList();
    }
    
    public async Task<List<BikeRentingHistory>> GetBikeLocationHistories(string email)
    {
        var manager = (await _unitOfWork.ManagerRepository.Find(x => x.Email == email)).FirstOrDefault()!;

        var bikeRentingHistories = (await _unitOfWork.BikeRentalTrackingRepository
                .Find(x =>
                    manager.IsSuperManager ||
                    x.Bike.BikeStation!.BikeStationManagers.Any(b => b.Manager.Email == email)
                    && x.IsActive
                ))
            .AsNoTracking()
            .Select(x => new BikeRentingHistory
            {
                Id = x.Id,
                BikeId = x.BikeId,
                BikePlate = x.Bike.LicensePlate,
                AccountEmail = x.Account.Email,
                BikeStationName = x.Bike.BikeStation!.Name,
                CheckedInOn = x.CheckinOn,
                CheckedOutOn = x.CheckoutOn,
                TotalTime = x.CheckoutOn.HasValue ? x.CheckoutOn.Value.Subtract(x.CheckinOn).TotalMinutes : null
            });

        return bikeRentingHistories.ToList();
    }

    private async Task<Bike> GetBikeById(int bikeId)
    {
        var bike = await _unitOfWork.BikeRepository.GetById(bikeId) ?? throw new BikeNotFoundException(bikeId);
        return bike ?? throw new BikeNotFoundException(bikeId);
    }

    private async Task StartTrackingBike(BikeCheckinDto bikeCheckinDto, string address, string accountEmail)
    {
        // var account = await GetAccountByEmail(userEmail);
        var bikeRentalTracking = await CreateBikeRentalBooking(
            bikeCheckinDto, accountEmail);
        var bikeTracking = (await _unitOfWork.BikeLocationTrackingRepository.Find(b =>
            b.BikeId == bikeCheckinDto.BikeId && b.IsActive == false)).FirstOrDefault();

        if (bikeTracking is null)
        {
            await _unitOfWork.BikeLocationTrackingRepository.Add(new BikeLocationTracking
            {
                BikeId = bikeCheckinDto.BikeId,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                IsActive = true,
                Longitude = bikeCheckinDto.Longitude,
                Latitude = bikeCheckinDto.Latitude,
                Address = address
            });
        }
        else
        {
            bikeTracking.UpdatedOn = DateTime.UtcNow;
            bikeTracking.IsActive = true;
            bikeTracking.Longitude = bikeCheckinDto.Longitude;
            bikeTracking.Latitude = bikeCheckinDto.Latitude;
            bikeTracking.Address = address;
        }
        
        await _unitOfWork.BikeRentalTrackingHistoryRepository.Add(new BikeLocationTrackingHistory
        {
            BikeId = bikeCheckinDto.BikeId,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            IsActive = true,
            Latitude = bikeCheckinDto.Latitude,
            Longitude = bikeCheckinDto.Longitude,
            Address = address,
            BikeRentalTracking = bikeRentalTracking
        });
    }
    
    private async Task StopTrackingBike(BikeCheckoutDto bikeCheckoutDto, string userEmail)
    {
        var finishBikeRentalTracking = await FinishBikeRentalBooking(bikeCheckoutDto, userEmail); 
        var bikeLocationTracking = (await _unitOfWork.BikeLocationTrackingRepository
            .Find(b => b.BikeId == bikeCheckoutDto.BikeId)).FirstOrDefault()
            ?? throw new UserHasNotRentAnyBikeException(userEmail);

        bikeLocationTracking.IsActive = false;
        bikeLocationTracking.UpdatedOn = DateTime.UtcNow;
        bikeLocationTracking.Address = bikeCheckoutDto.Address!;
        bikeLocationTracking.Latitude = bikeCheckoutDto.Latitude;
        bikeLocationTracking.Longitude = bikeCheckoutDto.Longitude;
        
        await _unitOfWork.BikeRentalTrackingHistoryRepository.Add(new BikeLocationTrackingHistory
        {
            BikeId = bikeCheckoutDto.BikeId,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            IsActive = true,
            Latitude = bikeCheckoutDto.Latitude,
            Longitude = bikeCheckoutDto.Longitude,
            Address = bikeCheckoutDto.Address!,
            BikeRentalTracking = finishBikeRentalTracking
        });
    }

    private async Task UpdateBikeCache(BikeCacheParameter bikeCacheParameter)
    {
        var key = string.Format(RedisCacheKey.SingleBike, bikeCacheParameter.BikeId);
        var bikeCache = await _cacheService.Get(key);

        if (bikeCache is null) return;
        var bike = JsonSerializer.Deserialize<BikeRetrieveDto>(bikeCache);
        ArgumentNullException.ThrowIfNull(bike);
        bike.LastLatitude = bikeCacheParameter.Latitude;
        bike.LastLongitude = bikeCacheParameter.Longitude;
        bike.LastAddress = bikeCacheParameter.Address;
        bike.Status = bikeCacheParameter.Status ?? bike.Status;
        bike.IsRenting = bikeCacheParameter.IsRenting ?? bike.IsRenting;
        
        await _cacheService.Add(key, JsonSerializer.Serialize(bike));
    }

    private async Task<BikeRentalTracking> CreateBikeRentalBooking(BikeCheckinDto bikeCheckinDto, string accountEmail)
    {
        var account = await GetAccountByEmail(accountEmail);
        var bikeRentalTracking = new BikeRentalTracking
        {
            CheckinOn = bikeCheckinDto.CheckinTime,
            AccountId = account.Id,
            BikeId = bikeCheckinDto.BikeId,
            IsActive = true,
            CreatedOn = DateTime.UtcNow
        };
        
        await _unitOfWork.BikeRentalTrackingRepository.Add(bikeRentalTracking);
        return bikeRentalTracking;
    }
    
    private async Task<BikeRentalTracking> FinishBikeRentalBooking(BikeCheckoutDto bikeCheckoutDto, string accountEmail)
    {
        var bikeRentalTracking = (await _unitOfWork.BikeRentalTrackingRepository.Find(b =>
            !b.CheckoutOn.HasValue && b.Account.Email == accountEmail))
            .OrderByDescending(b => b.CreatedOn).
            FirstOrDefault();
        
        ArgumentNullException.ThrowIfNull(bikeRentalTracking);
        bikeRentalTracking.CheckoutOn = bikeCheckoutDto.CheckoutOn;
        bikeRentalTracking.UpdatedOn = DateTime.UtcNow;
        bikeRentalTracking.TotalPoint = GetRentingPoint(bikeRentalTracking.CheckinOn, bikeRentalTracking.CheckoutOn.Value);

        return bikeRentalTracking;
    }

    private async Task UpdateBikeRentalTracking(BikeLocationDto bikeLocationDto)
    {
        var bikeRentalTracking = (await _unitOfWork.BikeLocationTrackingRepository.Find(
            b => b.BikeId == bikeLocationDto.BikeId)).FirstOrDefault();
        
        bikeRentalTracking!.Latitude = bikeLocationDto.Latitude;
        bikeRentalTracking.Longitude = bikeLocationDto.Longitude;
        bikeRentalTracking.UpdatedOn = DateTime.UtcNow;
        bikeRentalTracking.Address = bikeLocationDto.Address!;

        var latestBikeRentalHistory = (await _unitOfWork.BikeRentalTrackingHistoryRepository.Find(
            x => x.BikeId == bikeLocationDto.BikeId)).FirstOrDefault();
        
        await _unitOfWork.BikeRentalTrackingHistoryRepository.Add(new BikeLocationTrackingHistory
        {
            BikeId = bikeLocationDto.BikeId,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            IsActive = true,
            Latitude = bikeLocationDto.Latitude,
            Longitude = bikeLocationDto.Longitude,
            Address = bikeLocationDto.Address!,
            BikeRentalTrackingId = latestBikeRentalHistory!.BikeRentalTrackingId
        });
    }
    
    private async Task<Account> GetAccountByEmail(string email)
    {
        return (await _unitOfWork.AccountRepository.Find(a => a.Email == email)).FirstOrDefault() 
               ?? throw new AccountNotfoundException($"Account with email {email} not found!");
    }

    private static double GetRentingPoint(DateTime checkinOn, DateTime checkoutOn)
    {
        var duration = checkoutOn.Subtract(checkinOn).TotalHours;

        return duration switch
        {
            <= TimeDuration.TotalHourOfDay => duration * 2,
            <= TimeDuration.TotalHourOfWeek => duration * 1.0 / TimeDuration.TotalHourOfDay * 20,
            _ => duration * 1.0 / TimeDuration.TotalHourOfWeek * 100
        };
    }
}
