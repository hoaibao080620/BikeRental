using System.Text.Json;
using AutoMapper;
using BikeRental.MessageQueue.Commands;
using BikeRental.MessageQueue.MessageType;
using BikeService.Sonic.Const;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Dtos.BikeOperation;
using BikeService.Sonic.Exceptions;
using BikeService.Sonic.MessageQueue.Publisher;
using BikeService.Sonic.Models;
using BikeService.Sonic.Services.Interfaces;

namespace BikeService.Sonic.BusinessLogics;

public class BikeBusinessLogic : IBikeBusinessLogic
{
    private readonly IMapper _mapper;
    private readonly IBikeRepositoryAdapter _bikeRepositoryAdapter;
    private readonly IGoogleMapService _googleMapService;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageQueuePublisher _messageQueuePublisher;
    private readonly IBikeLocationHub _bikeLocationHub;

    public BikeBusinessLogic(
        IMapper mapper,
        IBikeRepositoryAdapter bikeRepositoryAdapter,
        IGoogleMapService googleMapService,
        ICacheService cacheService,
        IUnitOfWork unitOfWork,
        IMessageQueuePublisher messageQueuePublisher,
        IBikeLocationHub bikeLocationHub)
    {
        _mapper = mapper;
        _bikeRepositoryAdapter = bikeRepositoryAdapter;
        _googleMapService = googleMapService;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
        _messageQueuePublisher = messageQueuePublisher;
        _bikeLocationHub = bikeLocationHub;
    }

    public async Task<BikeRetrieveDto?> GetBike(int id)
    {
        return await _bikeRepositoryAdapter.GetBike(id);
    }

    public async Task<List<BikeRetrieveDto>> GetBikes(string managerEmail)
    {
        var bikes = await _bikeRepositoryAdapter.GetBikes(managerEmail);
        return bikes;
    }

    public async Task AddBike(BikeInsertDto bikeInsertDto)
    {
        var bike = _mapper.Map<Bike>(bikeInsertDto);
        bike.Status = BikeStatus.Available;
        await _unitOfWork.BikeRepository.Add(bike);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateBike(BikeUpdateDto bikeInsertDto)
    {
        var bike = _mapper.Map<Bike>(bikeInsertDto);
        bike.UpdatedOn = DateTime.UtcNow;
        await _unitOfWork.BikeRepository.Update(bike);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteBike(int id)
    {
        var bike = await _unitOfWork.BikeRepository.GetById(id);
        if (bike is null) throw new InvalidOperationException();
        
        await _unitOfWork.BikeRepository.Delete(bike);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task BikeChecking(BikeCheckinDto bikeCheckinDto, string accountEmail)
    {
        var managerEmails = await _unitOfWork.BikeStationManagerRepository
            .GetManagerEmailsByBikeId(bikeCheckinDto.BikeId);

        var bike = await GetBikeById(bikeCheckinDto.BikeId);
        var address = await _googleMapService.GetAddressOfLocation(
            bikeCheckinDto.Longitude,
            bikeCheckinDto.Latitude);
        
        var bikeStation = await _unitOfWork.BikeStationRepository.GetById(bikeCheckinDto.BikeStationId);
        bikeStation!.UsedParkingSpace++;

        await _bikeLocationHub.NotifyBikeLocationHasChanged(managerEmails.FirstOrDefault());
        var pushEventToMapTask = _messageQueuePublisher.PublishBikeLocationChangeCommand(managerEmails);
        var pushNotificationToManagers = _messageQueuePublisher.PublishBikeCheckinNotificationCommand(
            new PushBikeCheckinNotification
            {
                ManagerEmails = managerEmails,
                BikeId = bike.Id,
                BikeStationId = bikeStation.Id,
                BikeStationName = bikeStation.Name,
                AccountEmail = accountEmail,
                LicensePlate = bike.LicensePlate,
                CheckinOn = bikeCheckinDto.CheckinTime,
                MessageType = MessageType.NotifyBikeCheckin
            });
        
        var startTrackingBikeTask = StartTrackingBike(bikeCheckinDto);
        var createBikeBookingTask = CreateBikeRentalBooking(bikeCheckinDto, accountEmail);
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
            createBikeBookingTask,
            pushNotificationToManagers
        );
        
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task BikeCheckout(BikeCheckoutDto bikeCheckout, string accountEmail)
    {
        var bikeStation = await _unitOfWork.BikeStationRepository.GetById(bikeCheckout.BikeStationId);
        var managerEmails = await _unitOfWork.BikeStationManagerRepository
            .GetManagerEmailsByBikeId(bikeCheckout.BikeId);
        
        var bike = await GetBikeById(bikeCheckout.BikeId);
        bike.Status = BikeStatus.Available;
        bike.BikeStationId = bikeCheckout.BikeStationId;
        bikeStation!.UsedParkingSpace++;

        var address = await _googleMapService.GetAddressOfLocation(
            bikeCheckout.Longitude,
            bikeCheckout.Latitude);
        
        var pushEventToMapTask = _messageQueuePublisher.PublishBikeLocationChangeCommand(managerEmails);
        var stopTrackingBikeTask = StopTrackingBike(accountEmail, bike.Id);
        var updateBikeCache = UpdateBikeCache(new BikeCacheParameter
        {
            BikeId = bike.Id,
            Longitude = bikeCheckout.Longitude,
            Latitude = bikeCheckout.Latitude,
            Address = address,
            IsRenting = false,
            Status = BikeStatus.Available
        });
        
        var pushNotificationToManagers = _messageQueuePublisher.PublishBikeCheckoutNotificationCommand(
            new PushBikeCheckoutNotification
            {
                ManagerEmails = managerEmails,
                BikeId = bike.Id,
                BikeStationId = bikeStation.Id,
                BikeStationName = bikeStation.Name,
                AccountEmail = accountEmail,
                LicensePlate = bike.LicensePlate,
                CheckoutOn = bikeCheckout.CheckoutOn,
                MessageType = MessageType.NotifyBikeCheckout
            });
        
        await Task.WhenAll(pushEventToMapTask, stopTrackingBikeTask, updateBikeCache, pushNotificationToManagers);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateBikeLocation(BikeLocationDto bikeLocationDto)
    {
        var managerEmails = await _unitOfWork.BikeStationManagerRepository
            .GetManagerEmailsByBikeId(bikeLocationDto.BikeId);
        
        var bike = await GetBikeById(bikeLocationDto.BikeId);
        var bikeRentalTracking = (await _unitOfWork.BikeLocationTrackingRepository.Find(
            b => b.BikeId == bike.Id)).FirstOrDefault();
        
        bikeRentalTracking!.Latitude = bikeLocationDto.Latitude;
        bikeRentalTracking.Longitude = bikeLocationDto.Longitude;
        bikeRentalTracking.UpdatedOn = DateTime.UtcNow;
        
        bikeLocationDto.Operation = BikeLocationOperation.UpdateBikeFromMap;
        bikeLocationDto.Address = await _googleMapService.GetAddressOfLocation(
            bikeLocationDto.Longitude, 
            bikeLocationDto.Latitude);
        
        var pushEventTask = _messageQueuePublisher.PublishBikeLocationChangeCommand(managerEmails);
        var updateBikeCache = UpdateBikeCache(new BikeCacheParameter
        {
            BikeId = bike.Id,
            Longitude = bikeLocationDto.Longitude,
            Latitude = bikeLocationDto.Latitude,
            Address = bikeLocationDto.Address
        });

        await Task.WhenAll(pushEventTask, updateBikeCache);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<Bike> GetBikeById(int bikeId)
    {
        var bike = await _unitOfWork.BikeRepository.GetById(bikeId) ?? throw new BikeNotFoundException(bikeId);
        return bike ?? throw new BikeNotFoundException(bikeId);
    }

    private async Task StartTrackingBike(BikeCheckinDto bikeCheckinDto)
    {
        // var account = await GetAccountByEmail(userEmail);
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
            });
        }
        else
        {
            bikeTracking.UpdatedOn = DateTime.UtcNow;
            bikeTracking.IsActive = true;
            bikeTracking.Longitude = bikeCheckinDto.Longitude;
            bikeTracking.Latitude = bikeCheckinDto.Latitude;
        }
        
        await _unitOfWork.BikeRentalTrackingHistoryRepository.Add(new BikeLocationTrackingHistory
        {
            BikeId = bikeCheckinDto.BikeId,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            IsActive = true,
            Latitude = bikeCheckinDto.Latitude,
            Longitude = bikeCheckinDto.Longitude
        });
    }
    
    private async Task StopTrackingBike(string userEmail, int bikeId)
    {
        var bikeLocationTracking = (await _unitOfWork.BikeLocationTrackingRepository
            .Find(b => b.BikeId == bikeId)).FirstOrDefault()
            ?? throw new UserHasNotRentAnyBikeException(userEmail);

        bikeLocationTracking.IsActive = false;
        bikeLocationTracking.UpdatedOn = DateTime.UtcNow;
    }

    private async Task UpdateBikeCache(BikeCacheParameter bikeCacheParameter)
    {
        await _cacheService.Remove(string.Format(RedisCacheKey.SingleBikeStation, bikeCacheParameter.BikeId));
        var bikesCache = await _cacheService.Get(RedisCacheKey.BikeStationIds);

        if (bikesCache is null) return;

        var bikes = JsonSerializer.Deserialize<List<BikeRetrieveDto>>(bikesCache);
        var bike = bikes!.FirstOrDefault(b => b.Id == bikeCacheParameter.BikeId)!;
        bike.LastLatitude = bikeCacheParameter.Latitude;
        bike.LastLongitude = bikeCacheParameter.Longitude;
        bike.LastAddress = bikeCacheParameter.Address;
        bike.Status = bikeCacheParameter.Status ?? bike.Status;
        bike.IsRenting = bikeCacheParameter.IsRenting ?? bike.IsRenting;
        
        await _cacheService.Add(RedisCacheKey.BikeStationIds, JsonSerializer.Serialize(bikes));
    }

    private async Task CreateBikeRentalBooking(BikeCheckinDto bikeCheckinDto, string accountEmail)
    {
        var account = await GetAccountByEmail(accountEmail);
        await _unitOfWork.BikeRentalBookingRepository.Add(new BikeRentalBooking
        {
            CheckinOn = bikeCheckinDto.CheckinTime,
            AccountId = account.Id,
            BikeId = bikeCheckinDto.BikeId,
            IsActive = true,
            CreatedOn = DateTime.UtcNow
        });
    }
    
    private async Task<Account> GetAccountByEmail(string email)
    {
        return (await _unitOfWork.AccountRepository.Find(a => a.Email == email)).FirstOrDefault() 
               ?? throw new AccountNotfoundException($"Account with email {email} not found!");
    }
}
