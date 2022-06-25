using System.Text;
using System.Text.Json;
using AutoMapper;
using BikeService.Sonic.Const;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Dtos.BikeOperation;
using BikeService.Sonic.Exceptions;
using BikeService.Sonic.Models;
using BikeService.Sonic.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace BikeService.Sonic.BusinessLogics;

public class BikeBusinessLogic : IBikeBusinessLogic
{
    private readonly IBikeLocationHub _bikeLocationHub;
    private readonly IBikeStationManagerRepository _bikeStationManagerRepository;
    private readonly IBikeRepository _bikeRepository;
    private readonly IMapper _mapper;
    private readonly IBikeLocationTrackingRepository _bikeLocationTrackingRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IBikeRentalTrackingHistoryRepository _bikeRentalTrackingHistoryRepository;
    private readonly IBikeRepositoryAdapter _bikeRepositoryAdapter;
    private readonly IDistributedCache _distributedCache;
    private readonly IGoogleMapService _googleMapService;

    public BikeBusinessLogic(
        IBikeLocationHub bikeLocationHub, 
        IBikeStationManagerRepository bikeStationManagerRepository,
        IBikeRepository bikeRepository,
        IMapper mapper,
        IBikeLocationTrackingRepository bikeLocationTrackingRepository,
        IAccountRepository accountRepository,
        IBikeRentalTrackingHistoryRepository bikeRentalTrackingHistoryRepository,
        IBikeRepositoryAdapter bikeRepositoryAdapter,
        IDistributedCache distributedCache,
        IGoogleMapService googleMapService)
    {
        _bikeLocationHub = bikeLocationHub;
        _bikeStationManagerRepository = bikeStationManagerRepository;
        _bikeRepository = bikeRepository;
        _mapper = mapper;
        _bikeLocationTrackingRepository = bikeLocationTrackingRepository;
        _accountRepository = accountRepository;
        _bikeRentalTrackingHistoryRepository = bikeRentalTrackingHistoryRepository;
        _bikeRepositoryAdapter = bikeRepositoryAdapter;
        _distributedCache = distributedCache;
        _googleMapService = googleMapService;
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
        await _bikeRepository.Add(bike);
        await _bikeRepository.SaveChanges();
    }

    public async Task UpdateBike(BikeUpdateDto bikeInsertDto)
    {
        var bike = _mapper.Map<Bike>(bikeInsertDto);
        bike.UpdatedOn = DateTime.UtcNow;
        await _bikeRepository.Update(bike);
        await _bikeRepository.SaveChanges();
    }

    public async Task DeleteBike(int id)
    {
        var bike = await _bikeRepository.GetById(id);
        if (bike is null) throw new InvalidOperationException();
        
        await _bikeRepository.Delete(bike);
        await _bikeRepository.SaveChanges();
    }

    public async Task BikeChecking(BikeCheckinDto bikeCheckinDto, string userEmail)
    {
        var managerEmails = await _bikeStationManagerRepository.GetManagerEmailsByBikeId(bikeCheckinDto.BikeId);
        var bike = await GetBikeById(bikeCheckinDto.BikeId);
        var address = await _googleMapService.GetAddressOfLocation(
            bikeCheckinDto.Longitude,
            bikeCheckinDto.Latitude);

        var pushEventToMapTask = NotifyBikeLocationChange(managerEmails);
        var startTrackingBikeTask = StartTrackingBike(bikeCheckinDto, userEmail);
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
        await _bikeRepository.SaveChanges();
        await Task.WhenAll(pushEventToMapTask, startTrackingBikeTask, updateCachedTask);
    }

    public async Task BikeCheckout(BikeCheckoutDto bikeCheckout, string userEmail)
    {
        var managerEmails = await _bikeStationManagerRepository.GetManagerEmailsByBikeId(bikeCheckout.BikeId);
        var bike = await GetBikeById(bikeCheckout.BikeId);
        bike.Status = BikeStatus.Available;
        bike.BikeStationId = bikeCheckout.BikeStationId;
        await _bikeRepository.SaveChanges();
        var address = await _googleMapService.GetAddressOfLocation(
            bikeCheckout.Longitude,
            bikeCheckout.Latitude);
        
        var pushEventToMapTask = NotifyBikeLocationChange(managerEmails);
        var stopTrackingBikeTask = StopTrackingBike(userEmail, bike.Id);
        var updateBikeCache = UpdateBikeCache(new BikeCacheParameter
        {
            BikeId = bike.Id,
            Longitude = bikeCheckout.Longitude,
            Latitude = bikeCheckout.Latitude,
            Address = address,
            IsRenting = false,
            Status = BikeStatus.Available
        });
        
        await Task.WhenAll(pushEventToMapTask, stopTrackingBikeTask, updateBikeCache);
    }

    public async Task UpdateBikeLocation(BikeLocationDto bikeLocationDto)
    {
        var managerEmails = await _bikeStationManagerRepository.GetManagerEmailsByBikeId(bikeLocationDto.BikeId);
        var bike = await GetBikeById(bikeLocationDto.BikeId);
        var bikeRentalTracking = (await _bikeLocationTrackingRepository.Find(
            b => b.BikeId == bike.Id)).FirstOrDefault();
        
        bikeRentalTracking!.Latitude = bikeLocationDto.Latitude;
        bikeRentalTracking.Longitude = bikeLocationDto.Longitude;
        bikeRentalTracking.UpdatedOn = DateTime.UtcNow;
        await _bikeLocationTrackingRepository.SaveChanges();
        
        bikeLocationDto.LicensePlate = bike.LicensePlate;
        bikeLocationDto.Operation = BikeLocationOperation.UpdateBikeFromMap;
        bikeLocationDto.Address = await _googleMapService.GetAddressOfLocation(
            bikeLocationDto.Longitude, 
            bikeLocationDto.Latitude);
        
        var pushEventTask = NotifyBikeLocationChange(managerEmails);
        var updateBikeCache = UpdateBikeCache(new BikeCacheParameter
        {
            BikeId = bike.Id,
            Longitude = bikeLocationDto.Longitude,
            Latitude = bikeLocationDto.Latitude,
            Address = bikeLocationDto.Address
        });

        await Task.WhenAll(pushEventTask, updateBikeCache);
    }

    private async Task<Bike> GetBikeById(int bikeId)
    {
        var bike = await _bikeRepository.GetById(bikeId) ?? throw new BikeNotFoundException(bikeId);
        return bike ?? throw new BikeNotFoundException(bikeId);
    }

    private async Task NotifyBikeLocationChange(List<string> managerEmails)
    {
        foreach (var managerEmail in managerEmails)
        {
            await _bikeLocationHub.NotifyBikeLocationHasChanged(managerEmail);
        }
    }

    private async Task StartTrackingBike(BikeCheckinDto bikeCheckinDto, string userEmail)
    {
        // var account = await GetAccountByEmail(userEmail);
        var bikeTracking = (await _bikeLocationTrackingRepository.Find(b =>
            b.BikeId == bikeCheckinDto.BikeId && b.IsActive == false)).FirstOrDefault();

        if (bikeTracking is null)
        {
            await _bikeLocationTrackingRepository.Add(new BikeLocationTracking
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
        
        await _bikeRentalTrackingHistoryRepository.Add(new BikeLocationTrackingHistory
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
        var bikeLocationTracking = (await _bikeLocationTrackingRepository
            .Find(b => b.BikeId == bikeId)).FirstOrDefault()
            ?? throw new UserHasNotRentAnyBikeException(userEmail);

        bikeLocationTracking.IsActive = false;
        bikeLocationTracking.UpdatedOn = DateTime.UtcNow;
        await _bikeLocationTrackingRepository.SaveChanges();
    }

    private async Task<Account> GetAccountByEmail(string email)
    {
        return (await _accountRepository.Find(a => a.Email == email)).FirstOrDefault() 
            ?? throw new AccountNotfoundException($"Account with email {email} not found!");
    }

    private async Task UpdateBikeCache(BikeCacheParameter bikeCacheParameter)
    {
        await _distributedCache.RemoveAsync(string.Format(RedisCacheKey.SingleBikeStation, bikeCacheParameter.BikeId));
        var bikesCache = await _distributedCache.GetStringAsync(RedisCacheKey.BikeStationIds);

        if (bikesCache is null) return;

        var bikes = JsonSerializer.Deserialize<List<BikeRetrieveDto>>(bikesCache);
        var bike = bikes!.FirstOrDefault(b => b.Id == bikeCacheParameter.BikeId)!;
        bike.LastLatitude = bikeCacheParameter.Latitude;
        bike.LastLongitude = bikeCacheParameter.Longitude;
        bike.LastAddress = bikeCacheParameter.Address;
        bike.Status = bikeCacheParameter.Status ?? bike.Status;
        bike.IsRenting = bikeCacheParameter.IsRenting ?? bike.IsRenting;
        
        await _distributedCache.SetAsync(
            RedisCacheKey.BikeStationIds, 
            Encoding.ASCII.GetBytes( JsonSerializer.Serialize(bikes)), 
            new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(1),
                SlidingExpiration = TimeSpan.FromMinutes(45)
            });
    }
}
