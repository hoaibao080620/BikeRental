﻿using AutoMapper;
using BikeService.Sonic.Const;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Dtos.BikeOperation;
using BikeService.Sonic.Exceptions;
using BikeService.Sonic.Models;
using BikeService.Sonic.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BikeService.Sonic.BusinessLogics;

public class BikeBusinessLogic : IBikeBusinessLogic
{
    private readonly IBikeLocationHub _bikeLocationHub;
    private readonly IBikeStationManagerRepository _bikeStationManagerRepository;
    private readonly IBikeRepository _bikeRepository;
    private readonly IMapper _mapper;
    private readonly IBikeRentalTrackingRepository _bikeRentalTrackingRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IBikeStationBusinessLogic _bikeStationBusinessLogic;
    private readonly IBikeStationRepository _bikeStationRepository;
    private readonly IBikeRentalTrackingHistoryRepository _bikeRentalTrackingHistoryRepository;

    public BikeBusinessLogic(
        IBikeLocationHub bikeLocationHub, 
        IBikeStationManagerRepository bikeStationManagerRepository,
        IBikeRepository bikeRepository,
        IMapper mapper,
        IBikeRentalTrackingRepository bikeRentalTrackingRepository,
        IAccountRepository accountRepository,
        IBikeStationBusinessLogic bikeStationBusinessLogic,
        IBikeStationRepository bikeStationRepository,
        IBikeRentalTrackingHistoryRepository bikeRentalTrackingHistoryRepository)
    {
        _bikeLocationHub = bikeLocationHub;
        _bikeStationManagerRepository = bikeStationManagerRepository;
        _bikeRepository = bikeRepository;
        _mapper = mapper;
        _bikeRentalTrackingRepository = bikeRentalTrackingRepository;
        _accountRepository = accountRepository;
        _bikeStationBusinessLogic = bikeStationBusinessLogic;
        _bikeStationRepository = bikeStationRepository;
        _bikeRentalTrackingHistoryRepository = bikeRentalTrackingHistoryRepository;
    }

    public async Task<BikeRetrieveDto?> GetBike(int id)
    {
        var bike = await _bikeRepository.Find(b => b.Id == id);
        return bike.AsNoTracking().Select(b => new BikeRetrieveDto
        {
            BikeStationId = b.BikeStationId,
            BikeStationName = b.BikeStation != null ? b.BikeStation.Name : null,
            Id = b.Id,
            CreatedOn = b.CreatedOn,
            IsActive = b.IsActive,
            Description = b.Description,
            LicensePlate = b.LicensePlate,
            Status = b.Status,
            UpdatedOn = b.UpdatedOn
        }).FirstOrDefault();
    }

    public async Task<List<BikeRetrieveDto>> GetBikes(string managerEmail)
    {
        var bikes = await _bikeRepository.All();
        return bikes.Where(b => 
            b.BikeStation != null && 
            b.BikeStation.BikeStationManagers.Any(bs => bs.Manager.Email == managerEmail))
        .Select(b => new BikeRetrieveDto
        {
            BikeStationId = b.BikeStationId,
            BikeStationName = b.BikeStation != null ? b.BikeStation.Name : null,
            Id = b.Id,
            CreatedOn = b.CreatedOn,
            IsActive = b.IsActive,
            Description = b.Description,
            LicensePlate = b.LicensePlate,
            Status = b.Status,
            UpdatedOn = b.UpdatedOn
        }).AsNoTracking().ToList();
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
        bike.Status = BikeStatus.InUsed;
        await _bikeRepository.SaveChanges();
        await Task.WhenAll(pushEventToMapTask, startTrackingBikeTask);
    }

    public async Task BikeCheckout(BikeCheckoutDto bikeCheckout, string userEmail)
    {
        var managerEmails = await _bikeStationManagerRepository.GetManagerEmailsByBikeId(bikeCheckout.BikeId);
        var bike = await GetBikeById(bikeCheckout.BikeId);
        bike.Status = BikeStatus.Available;
        await _bikeRepository.SaveChanges();
        
        var pushEventToMapTask = PushEventToMap(managerEmails, new BikeLocationDto
        {
            BikeId = bike.Id,
            Operation = BikeLocationOperation.RemoveBikeFromMap
        });
        
        var stopTrackingBikeTask = StopTrackingBike(userEmail);
        var assignBikeToBikeStationTask = AssignBikeToNearestBikeStation(bikeCheckout, bike);
        await Task.WhenAll(pushEventToMapTask, stopTrackingBikeTask, assignBikeToBikeStationTask);
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
        var bike = await _bikeRepository.GetById(bikeId) ?? throw new BikeNotFoundException(bikeId);
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

        await _bikeRentalTrackingHistoryRepository.Add(new BikeRentalTrackingHistory
        {
            BikeId = bikeCheckinDto.BikeId,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            IsActive = true,
            Latitude = bikeCheckinDto.Latitude,
            Longitude = bikeCheckinDto.Longitude
        });
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
    
    private async Task AssignBikeToNearestBikeStation(BikeCheckoutDto bikeCheckout, Bike bike)
    {
        var nearestBikeStation = await _bikeStationBusinessLogic
            .GetNearestBikeStationFromLocation(bikeCheckout.Longitude, bikeCheckout.Latitude);

        bike.BikeStationId = nearestBikeStation.Id;
        await _bikeRepository.SaveChanges();
    }
}
