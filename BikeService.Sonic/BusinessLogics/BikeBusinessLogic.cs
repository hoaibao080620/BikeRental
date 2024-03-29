﻿using AutoMapper;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.MessageType;
using BikeService.Sonic.Const;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Exceptions;
using BikeService.Sonic.MessageQueue.Publisher;
using BikeService.Sonic.Models;
using BikeService.Sonic.Services.Interfaces;
using Grpc.Net.ClientFactory;
using Microsoft.EntityFrameworkCore;

namespace BikeService.Sonic.BusinessLogics;

public class BikeBusinessLogic : IBikeBusinessLogic
{
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageQueuePublisher _messageQueuePublisher;
    private readonly BikeBookingServiceGrpc.BikeBookingServiceGrpcClient _bookingClient;

    public BikeBusinessLogic(
        IMapper mapper,
        ICacheService cacheService,
        IUnitOfWork unitOfWork,
        IMessageQueuePublisher messageQueuePublisher,
        GrpcClientFactory grpcClientFactory)
    {
        _mapper = mapper;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
        _messageQueuePublisher = messageQueuePublisher;
        _bookingClient =
            grpcClientFactory.CreateClient<BikeBookingServiceGrpc.BikeBookingServiceGrpcClient>("BikeBooking");
    }

    public async Task<BikeRetrieveDto?> GetBike(string bikeCode)
    {
        var bike = await (await _unitOfWork.BikeRepository.Find(x => x.BikeCode.ToLower() == bikeCode.ToLower()))
            .AsNoTracking().Select(b => new BikeRetrieveDto
            {
                BikeStationId = b.BikeStationId,
                BikeStationName = b.BikeStation != null ? b.BikeStation.Name : null,
                Id = b.Id,
                Description = b.Description,
                LicensePlate = b.BikeCode,
                Status = b.Status,
                UpdatedOn = b.UpdatedOn
            }).FirstOrDefaultAsync() ?? throw new BikeNotFoundException(bikeCode);

        var bikeRenting = await _bookingClient.GetBikesRentingCountAsync(new GetBikesRentingCountRequest
        {
            Ids = {bike.Id}
        });

        bike.RentingCount = bikeRenting.BikesRentingCount.Any() ? bikeRenting.BikesRentingCount.First().RentingCount : default;
        
        return bike;
    }

    public async Task<List<BikeRetrieveDto>> GetBikes(string managerEmail)
    {
        var manager = await _unitOfWork.ManagerRepository.Exists(x => x.Email == managerEmail);
        
        var isSuperManager =
            await _unitOfWork.ManagerRepository.Exists(x => x.Email == managerEmail && x.IsSuperManager);
        var bikes = (await _unitOfWork.BikeRepository
                .Find(x => isSuperManager || x.BikeStation != null && x.BikeStation.BikeStationManagers
                    .Any(b => b.Manager.Email == managerEmail) || !manager))
                .AsNoTracking().Select(b => new BikeRetrieveDto
                {
                    BikeStationId = b.BikeStationId,
                    BikeStationName = b.BikeStation != null ? b.BikeStation.Name : null,
                    Id = b.Id,
                    Description = b.Description,
                    LicensePlate = b.BikeCode,
                    Status = b.Status,
                    UpdatedOn = b.UpdatedOn,
                    CreatedOn = b.CreatedOn
                }).OrderByDescending(x => x.UpdatedOn).ThenByDescending(x => x.CreatedOn).ToList();

        var bikesRentingCount = await _bookingClient.GetBikesRentingCountAsync(new GetBikesRentingCountRequest
        {
            Ids = {bikes.Select(x => x.Id)}
        });

        var bikesRentingCountDict = bikesRentingCount.BikesRentingCount
            .ToList()
            .ToDictionary(x => x.BikeId, x => x.RentingCount);

        foreach (var bike in bikes)
        {
            bike.RentingCount = bikesRentingCountDict.GetValueOrDefault(bike.Id);
        }

        return bikes;
    }

    public async Task AddBike(BikeInsertDto bikeInsertDto)
    {
        var bikeStationColor = (await _unitOfWork.BikeStationColorRepository
            .Find(x => x.BikeStationId == bikeInsertDto.BikeStationId)).FirstOrDefault();
            
        var bike = _mapper.Map<Bike>(bikeInsertDto);
        bike.Status = BikeStatus.Available;
        await _unitOfWork.BikeRepository.Add(bike);
        bike.CreatedOn = DateTime.UtcNow;
        bike.IsActive = true;
        bike.BikeCode = "Temp";
        bike.UpdatedOn = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
        bike.BikeCode = $"BR-{bike.Id.ToString().PadLeft(6, '0')}";
        await _unitOfWork.SaveChangesAsync();
        var bikeStation = bike.BikeStationId.HasValue ?
            await _unitOfWork.BikeStationRepository.GetById(bike.BikeStationId.Value) : null;
        await _messageQueuePublisher.PublishBikeCreatedEvent(new BikeCreated
        {
            Id = bike.Id,
            BikeStationId = bike.BikeStationId,
            BikeStationName = bikeStation?.Name,
            Description = bike.Description,
            LicensePlate = bike.BikeCode,
            Status = bike.Status,
            MessageType = MessageType.BikeCreated,
            Color = bikeStationColor?.Color,
            BikeStationCode = bikeStation?.Code
        });
    }

    public async Task UpdateBike(BikeUpdateDto bikeInsertDto)
    {
        var bikeFromDb = await _unitOfWork.BikeRepository.GetById(bikeInsertDto.Id) ?? throw new ArgumentException();
        _mapper.Map(bikeInsertDto, bikeFromDb);
        bikeFromDb.UpdatedOn = DateTime.UtcNow;
        var bikeStation = bikeFromDb.BikeStationId.HasValue ?
            await _unitOfWork.BikeStationRepository.GetById(bikeFromDb.BikeStationId.Value) : null;
        await _cacheService.Remove(string.Format(RedisCacheKey.SingleBike, bikeFromDb.Id));
        await _unitOfWork.BikeRepository.Update(bikeFromDb);
        await _unitOfWork.SaveChangesAsync();

        var bikeStationColor = (await _unitOfWork.BikeStationColorRepository
            .Find(x => x.BikeStationId == bikeFromDb.BikeStationId)).FirstOrDefault();
        await _messageQueuePublisher.PublishBikeUpdatedEvent(new BikeUpdated
        {
            Id = bikeFromDb.Id,
            BikeStationId = bikeFromDb.BikeStationId,
            BikeStationName = bikeStation?.Name,
            Description = bikeFromDb.Description,
            LicensePlate = bikeFromDb.BikeCode,
            MessageType = MessageType.BikeUpdated,
            BikeStationCode = bikeStation?.Code,
            Color = bikeStationColor?.Color
        });
    }

    public async Task DeleteBike(int id)
    {
        var bike = await _unitOfWork.BikeRepository.GetById(id);
        if (bike is null) throw new InvalidOperationException();
        
        if (bike.Status == BikeStatus.InUsed)
            throw new InvalidOperationException("Không thể xóa xe đang trong quá trình sử dụng!");

        var bikeLocationHistories = (await _unitOfWork.BikeReportRepository
            .Find(x => x.BikeId == id)).ToList();

        foreach (var bikeLocationHistory in bikeLocationHistories)
        {
            await _unitOfWork.BikeReportRepository.Delete(bikeLocationHistory);
        }
        
        await _unitOfWork.BikeRepository.Delete(bike);
        await _unitOfWork.SaveChangesAsync();
        await _messageQueuePublisher.PublishBikeDeletedEvent(id);
    }

    public async Task DeleteBikes(List<int> bikeIds)
    {
        var bikes = (await _unitOfWork.BikeRepository.Find(x => bikeIds.Contains(x.Id))).ToList();
        if (bikes.Any(x => x.Status == BikeStatus.InUsed || x.BikeStationId.HasValue))
            throw new InvalidOperationException("Một trong những xe bạn xóa có xe đang trong quá trình " +
                                                "sử dụng hoặc đang thuộc về trạm!");
        
        var bikeLocationHistories = (await _unitOfWork.BikeReportRepository
            .Find(x => bikeIds.Contains(x.BikeId))).ToList();

        foreach (var bikeLocationHistory in bikeLocationHistories)
        {
            await _unitOfWork.BikeReportRepository.Delete(bikeLocationHistory);
        }
        
        foreach (var bike in bikes)
        {
            await _unitOfWork.BikeRepository.Delete(bike);
            await _unitOfWork.SaveChangesAsync();
            await _messageQueuePublisher.PublishBikeDeletedEvent(bike.Id);
        }
    }

    public async Task UnlockBike(int bikeId)
    {
        var bike = await _unitOfWork.BikeRepository.GetById(bikeId);
        if (bike is null) return;
        bike.IsLock = true;
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<int> GetCurrentRentingBike(string phoneNumber)
    {
        var currentBikeRenting = await _bookingClient.GetCurrentRentingBikeAsync(new GetCurrentRentingBikeRequest
        {
            PhoneNumber = phoneNumber
        });

        return currentBikeRenting.BikeId;
    }

    public async Task<List<BikeRetrieveDto>> GetAllBikes()
    {
        var bikes = (await _unitOfWork.BikeRepository
                .All())
            .AsNoTracking().Select(b => new BikeRetrieveDto
            {
                BikeStationId = b.BikeStationId,
                BikeStationName = b.BikeStation != null ? b.BikeStation.Name : null,
                Id = b.Id,
                Description = b.Description,
                LicensePlate = b.BikeCode,
                Status = b.Status,
                UpdatedOn = b.UpdatedOn,
                CreatedOn = b.CreatedOn
            }).OrderByDescending(x => x.UpdatedOn).ThenByDescending(x => x.CreatedOn).ToList();

        var bikesRentingCount = await _bookingClient.GetBikesRentingCountAsync(new GetBikesRentingCountRequest
        {
            Ids = {bikes.Select(x => x.Id)}
        });

        var bikesRentingCountDict = bikesRentingCount.BikesRentingCount
            .ToList()
            .ToDictionary(x => x.BikeId, x => x.RentingCount);

        foreach (var bike in bikes)
        {
            bike.RentingCount = bikesRentingCountDict.GetValueOrDefault(bike.Id);
        }

        return bikes;
    }
}
