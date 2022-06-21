using AutoMapper;
using BikeService.Sonic.Const;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Dtos.BikeOperation;
using BikeService.Sonic.Exceptions;
using BikeService.Sonic.Models;
using BikeService.Sonic.Services.Interfaces;

namespace BikeService.Sonic.BusinessLogics;

public class BikeBusinessLogic : IBikeBusinessLogic
{
    private readonly IBikeLocationHub _bikeLocationHub;
    private readonly IBikeStationManagerRepository _bikeStationManagerRepository;
    private readonly IBikeRepository _bikeRepository;
    private readonly IMapper _mapper;
    private readonly IBikeRentalTrackingRepository _bikeRentalTrackingRepository;
    private readonly IAccountRepository _accountRepository;

    public BikeBusinessLogic(
        IBikeLocationHub bikeLocationHub, 
        IBikeStationManagerRepository bikeStationManagerRepository,
        IBikeRepository bikeRepository,
        IMapper mapper,
        IBikeRentalTrackingRepository bikeRentalTrackingRepository,
        IAccountRepository accountRepository)
    {
        _bikeLocationHub = bikeLocationHub;
        _bikeStationManagerRepository = bikeStationManagerRepository;
        _bikeRepository = bikeRepository;
        _mapper = mapper;
        _bikeRentalTrackingRepository = bikeRentalTrackingRepository;
        _accountRepository = accountRepository;
    }

    public async Task<BikeRetrieveDto> GetBike(int id)
    {
        var bike = await GetBikeById(id);
        return _mapper.Map<BikeRetrieveDto>(bike);
    }

    public async Task<List<BikeRetrieveDto>> GetBikes()
    {
        var bikes = await _bikeRepository.All();
        return _mapper.Map<List<BikeRetrieveDto>>(bikes);
    }

    public async Task AddBike(BikeInsertDto bikeInsertDto)
    {
        var bike = _mapper.Map<Bike>(bikeInsertDto);
        bike.Status = "Available";
        await _bikeRepository.Add(bike);
        await _bikeRepository.SaveChanges();
    }

    public async Task UpdateBike(BikeUpdateDto bikeInsertDto)
    {
        await _bikeRepository.Update(_mapper.Map<Bike>(bikeInsertDto));
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

        var bikeLocation = new BikeLocationDto
        {
            BikeId = bike.Id,
            Longitude = bikeCheckinDto.Longitude,
            Latitude = bikeCheckinDto.Latitude,
            Plate = bike.LicensePlate,
            Operation = BikeLocationOperation.AddBikeToMap
        };

        var pushEventToMapTask = PushEventToMap(managerEmails, bikeLocation);
        var startTrackingBikeTask = StartTrackingBike(bikeCheckinDto, userEmail);

        await Task.WhenAll(pushEventToMapTask, startTrackingBikeTask);
    }

    public async Task BikeCheckout(BikeCheckoutDto bikeCheckingDto, string userEmail)
    {
        var managerEmails = await _bikeStationManagerRepository.GetManagerEmailsByBikeId(bikeCheckingDto.BikeId);
        var bike = await GetBikeById(bikeCheckingDto.BikeId);
        
        var pushEventToMapTask = PushEventToMap(managerEmails, new BikeLocationDto
        {
            BikeId = bike.Id,
            Operation = BikeLocationOperation.RemoveBikeFromMap
        });
        
        var stopTrackingBikeTask = StopTrackingBike(userEmail);
        await Task.WhenAll(pushEventToMapTask, stopTrackingBikeTask);
    }

    public async Task UpdateBikeLocation(BikeLocationDto bikeLocationDto)
    {
        var managerEmails = await _bikeStationManagerRepository.GetManagerEmailsByBikeId(bikeLocationDto.BikeId);
        var bike = await GetBikeById(bikeLocationDto.BikeId);

        bikeLocationDto.Plate = bike.LicensePlate;
        bikeLocationDto.Operation = BikeLocationOperation.UpdateBikeFromMap;
        await PushEventToMap(managerEmails, bikeLocationDto);
    }

    private async Task<Bike> GetBikeById(int bikeId)
    {
        var bike = await _bikeRepository.GetById(bikeId);
        return bike ?? throw new BikeNotFoundException(bikeId);
    }

    private async Task PushEventToMap(List<string> managerEmails, BikeLocationDto bikeLocationDto)
    {
        foreach (var managerEmail in managerEmails)
        {
            await _bikeLocationHub.SendBikeLocationsData(managerEmail, bikeLocationDto);
        }
    }

    private async Task StartTrackingBike(BikeCheckinDto bikeCheckinDto, string userEmail)
    {
        var account = await GetAccountByEmail(userEmail);
        await _bikeRentalTrackingRepository.Add(new BikeRentalOrder
        {
            BikeId = bikeCheckinDto.BikeId,
            AccountId = account.Id,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            IsActive = true,
            Longitude = bikeCheckinDto.Longitude,
            Latitude = bikeCheckinDto.Latitude,
            CheckinTime = bikeCheckinDto.CheckinTime
        });

        await _bikeRentalTrackingRepository.SaveChanges();
    }
    
    private async Task StopTrackingBike(string userEmail)
    {
        var bikeRentalTracking = (await _bikeRentalTrackingRepository
            .Find(b => b.Account.Email == userEmail)).FirstOrDefault()
            ?? throw new UserHasNotRentAnyBikeException(userEmail);
        
        await _bikeRentalTrackingRepository.Delete(bikeRentalTracking);
        await _bikeRentalTrackingRepository.SaveChanges();
    }

    private async Task<Account> GetAccountByEmail(string email)
    {
        return (await _accountRepository.Find(a => a.Email == email)).FirstOrDefault() 
            ?? throw new AccountNotfoundException($"Account with email {email} not found!");
    }
}
