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
                LicensePlate = b.BikeCode,
                Status = b.Status,
                UpdatedOn = b.UpdatedOn
            }).FirstOrDefaultAsync() ?? throw new BikeNotFoundException(id);
        
        return bike;
    }

    public async Task<List<BikeRetrieveDto>> GetBikes(string managerEmail)
    {
        var isSuperManager =
            await _unitOfWork.ManagerRepository.Exists(x => x.Email == managerEmail && x.IsSuperManager);
        var bikes = (await _unitOfWork.BikeRepository
                .Find(x => isSuperManager || x.BikeStation != null && x.BikeStation.BikeStationManagers
                    .Any(b => b.Manager.Email == managerEmail)))
                .AsNoTracking().Select(b => new BikeRetrieveDto
                {
                    BikeStationId = b.BikeStationId,
                    BikeStationName = b.BikeStation != null ? b.BikeStation.Name : null,
                    Id = b.Id,
                    Description = b.Description,
                    LicensePlate = b.BikeCode,
                    Status = b.Status,
                    UpdatedOn = b.UpdatedOn
                }).OrderByDescending(x => x.UpdatedOn).ToList();
        
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

        var bikeStationColor = (await _unitOfWork.BikeStationColorRepository
            .Find(x => x.BikeStationId == bike.BikeStationId)).FirstOrDefault();
        await _messageQueuePublisher.PublishBikeUpdatedEvent(new BikeUpdated
        {
            Id = bike.Id,
            BikeStationId = bike.BikeStationId,
            BikeStationName = bikeStation?.Name,
            Description = bike.Description,
            LicensePlate = bike.BikeCode,
            MessageType = MessageType.BikeUpdated,
            Color = bikeStationColor?.Color
        });
    }

    public async Task DeleteBike(int id)
    {
        var bike = await _unitOfWork.BikeRepository.GetById(id);
        if (bike is null) throw new InvalidOperationException();
        
        if (bike.Status == BikeStatus.InUsed || bike.BikeStationId.HasValue)
            throw new InvalidOperationException("Không thể xóa xe đang trong quá trình sử dụng hoặc đang thuộc về trạm!");
        
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

    private async Task<Bike> GetBikeById(int bikeId)
    {
        var bike = await _unitOfWork.BikeRepository.GetById(bikeId) ?? throw new BikeNotFoundException(bikeId);
        return bike ?? throw new BikeNotFoundException(bikeId);
    }
}
