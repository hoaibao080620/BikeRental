using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.MessageType;
using BikeService.Sonic.Dtos.BikeOperation;
using BikeTrackingService.DAL;
using BikeTrackingService.Dtos;
using BikeTrackingService.Dtos.BikeOperation;
using BikeTrackingService.Dtos.History;
using BikeTrackingService.MessageQueue.Publisher;
using BikeTrackingService.Models;
using BikeTrackingService.Services;
using Grpc.Net.ClientFactory;
using Microsoft.EntityFrameworkCore;

namespace BikeTrackingService.BLL;

public class BikeTrackingBusinessLogic : IBikeTrackingBusinessLogic
{
    private readonly BikeServiceGrpc.BikeServiceGrpcClient _bikeServiceGrpc;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGoogleMapService _googleMapService;
    private readonly IMessageQueuePublisher _messageQueuePublisher;

    public BikeTrackingBusinessLogic(
        GrpcClientFactory grpcClientFactory,
        IUnitOfWork unitOfWork,
        IGoogleMapService googleMapService,
        IMessageQueuePublisher messageQueuePublisher)
    {
        _bikeServiceGrpc = grpcClientFactory.CreateClient<BikeServiceGrpc.BikeServiceGrpcClient>("BikeService");
        _unitOfWork = unitOfWork;
        _googleMapService = googleMapService;
        _messageQueuePublisher = messageQueuePublisher;
    }
    
    public async Task<List<BikeRentingHistory>> GetBikeRentingHistories(string email)
    {
        var getBikeIdsResponse = await _bikeServiceGrpc.GetBikeIdsOfManagerAsync(new GetBikeIdsRequest
        {
            ManagerEmail = email
        });
        ArgumentNullException.ThrowIfNull(getBikeIdsResponse);

        var bikeRentingHistories = (await _unitOfWork.BikeRentalTrackingRepository
            .Find(x =>
                getBikeIdsResponse.BikeIds.Contains(x.BikeId)
            )).Select(x => new BikeRentingHistory
            {
                Id = x.Id,
                BikeId = x.BikeId,
                BikePlate = x.Bike.LicensePlate,
                AccountEmail = x.Account.Email,
                CheckedInOn = x.CheckinOn,
                CheckedOutOn = x.CheckoutOn,
                AccountPhone = x.Account.PhoneNumber,
                TotalTime = x.CheckoutOn.HasValue ? x.CheckoutOn.Value.Subtract(x.CheckinOn).TotalMinutes : null
            }).ToList();

        return bikeRentingHistories;
    }

    public async Task<List<BikeTrackingRetrieveDto>> GetBikesTracking(string email)
    {
        var getBikeIdsResponse = await _bikeServiceGrpc.GetBikeIdsOfManagerAsync(new GetBikeIdsRequest
        {
            ManagerEmail = email
        });
        ArgumentNullException.ThrowIfNull(getBikeIdsResponse);

        var bikesTracking = (await _unitOfWork.BikeRepository
                .Find(x => getBikeIdsResponse.BikeIds.Contains(x.Id)))
            .AsNoTracking()
            .Select(x => new BikeTrackingRetrieveDto
            {
                BikeId = x.Id,
                LicensePlate = x.LicensePlate,
                BikeStationColor = x.Color,
                BikeStationId = x.BikeStationId,
                BikeStationName = x.BikeStationName,
                Description = x.Description,
                IsRenting = x.BikeLocationTrackings.Any(b => b.IsActive && b.BikeId == x.Id),
                LastAddress = x.BikeLocationTrackings.Any(b => b.IsActive && b.BikeId == x.Id) ?
                    x.BikeLocationTrackings.First(b => b.IsActive && b.BikeId == x.Id).Address : null,
                LastLatitude = x.BikeLocationTrackings.Any(b => b.IsActive && b.BikeId == x.Id) ?
                    x.BikeLocationTrackings.First(b => b.IsActive && b.BikeId == x.Id).Latitude : null,
                LastLongitude  = x.BikeLocationTrackings.Any(b => b.IsActive && b.BikeId == x.Id) ?
                    x.BikeLocationTrackings.First(b => b.IsActive && b.BikeId == x.Id).Longitude : null,
            }).ToList();

        return bikesTracking;
    }

    public async Task BikeChecking(BikeCheckinDto bikeCheckinDto, string accountEmail)
    {
        var managerEmails = (await _bikeServiceGrpc.GetManagerEmailsOfBikeIdAsync(new GetManagerEmailsRequest
        {
            BikeId = bikeCheckinDto.BikeId
        })).ManagerEmails.ToList();

        var bike = await GetBikeById(bikeCheckinDto.BikeId);
        var address = await _googleMapService.GetAddressOfLocation(
            bikeCheckinDto.Longitude,
            bikeCheckinDto.Latitude);
        ArgumentNullException.ThrowIfNull(address);
        
        var pushEventToMapTask = _messageQueuePublisher.PublishBikeLocationChangeCommand(managerEmails);
        var pushNotificationToManagers = _messageQueuePublisher.PublishBikeCheckedInEvent(
            new BikeCheckedIn
            {
                ManagerEmails = managerEmails,
                BikeId = bike.Id,
                BikeStationId = bike.BikeStationId!.Value,
                BikeStationName = bike.BikeStationName!,
                AccountEmail = accountEmail,
                LicensePlate = bike.LicensePlate,
                CheckinOn = bikeCheckinDto.CheckinTime,
                MessageType = MessageType.BikeCheckedIn
            });
        
        var startTrackingBikeTask = StartTrackingBike(bikeCheckinDto, address, accountEmail);
        // var updateCachedTask = UpdateBikeCache(new BikeCacheParameter
        // {
        //     BikeId = bike.Id,
        //     Longitude = bikeCheckinDto.Longitude,
        //     Latitude = bikeCheckinDto.Latitude,
        //     IsRenting = true,
        //     Address = address,
        //     Status = BikeStatus.InUsed
        // });
        
        await Task.WhenAll(
            pushEventToMapTask, 
            startTrackingBikeTask,
            pushNotificationToManagers
        );
        
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task BikeCheckout(BikeCheckoutDto bikeCheckout, string accountEmail)
    {
        var managerEmails = (await _bikeServiceGrpc.GetManagerEmailsOfBikeIdAsync(new GetManagerEmailsRequest
        {
            BikeId = bikeCheckout.BikeId
        })).ManagerEmails.ToList();
        
        var bike = await GetBikeById(bikeCheckout.BikeId);
        var address = await _googleMapService.GetAddressOfLocation(
            bikeCheckout.Longitude,
            bikeCheckout.Latitude);
        
        ArgumentNullException.ThrowIfNull(address);

        var pushEventToMapTask = _messageQueuePublisher.PublishBikeLocationChangeCommand(managerEmails);
        var stopTrackingBikeTask = StopTrackingBike(bikeCheckout, accountEmail, address);
        // var updateBikeCache = UpdateBikeCache(new BikeCacheParameter
        // {
        //     BikeId = bike.Id,
        //     Longitude = bikeCheckout.Longitude,
        //     Latitude = bikeCheckout.Latitude,
        //     Address = address,
        //     IsRenting = false,
        //     Status = BikeStatus.Available
        // });
        
        var pushNotificationToManagers = _messageQueuePublisher.PublishBikeCheckedOutEvent(
            new BikeCheckedOut
            {
                ManagerEmails = managerEmails,
                BikeId = bike.Id,
                BikeStationId = bike.BikeStationId!.Value,
                BikeStationName = bike.BikeStationName!,
                AccountEmail = accountEmail,
                LicensePlate = bike.LicensePlate,
                CheckoutOn = bikeCheckout.CheckoutOn,
                MessageType = MessageType.BikeCheckedOut
            });
        
        await Task.WhenAll(
            pushEventToMapTask, 
            stopTrackingBikeTask, 
            pushNotificationToManagers);
        
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateBikeLocation(BikeLocationDto bikeLocationDto)
    {
        var managerEmails = (await _bikeServiceGrpc.GetManagerEmailsOfBikeIdAsync(new GetManagerEmailsRequest
        {
            BikeId = bikeLocationDto.BikeId
        })).ManagerEmails.ToList();
        
        // var bike = await GetBikeById(bikeLocationDto.BikeId);
        var address = await _googleMapService.GetAddressOfLocation(
            bikeLocationDto.Longitude, 
            bikeLocationDto.Latitude);
        
        ArgumentNullException.ThrowIfNull(address);

        var updateBikeTracking = UpdateBikeRentalTracking(bikeLocationDto, address);
        
        var pushEventTask = _messageQueuePublisher.PublishBikeLocationChangeCommand(managerEmails);
        // var updateBikeCache = UpdateBikeCache(new BikeCacheParameter
        // {
        //     BikeId = bike.Id,
        //     Longitude = bikeLocationDto.Longitude,
        //     Latitude = bikeLocationDto.Latitude,
        //     Address = bikeLocationDto.Address
        // });

        await Task.WhenAll(pushEventTask, updateBikeTracking);
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

    private async Task<Bike> GetBikeById(int bikeId)
    {
        var bike = await _unitOfWork.BikeRepository.GetById(bikeId) ?? throw new InvalidOperationException();
        return bike;
    }
    
    private async Task StartTrackingBike(BikeCheckinDto bikeCheckinDto, string address, string accountEmail)
    {
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
        
        await _unitOfWork.BikeLocationTrackingHistoryRepository.Add(new BikeLocationTrackingHistory
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
    
    private async Task<Account> GetAccountByEmail(string email)
    {
        return (await _unitOfWork.AccountRepository.Find(a => a.Email == email)).FirstOrDefault() 
               ?? throw new InvalidOperationException($"Account with email {email} not found!");
    }
    
    private async Task StopTrackingBike(BikeCheckoutDto bikeCheckoutDto, string userEmail, string address)
    {
        var finishBikeRentalTracking = await FinishBikeRentalBooking(bikeCheckoutDto, userEmail); 
        var bikeLocationTracking = (await _unitOfWork.BikeLocationTrackingRepository
                                       .Find(b => b.BikeId == bikeCheckoutDto.BikeId)).FirstOrDefault()
                                   ?? throw new InvalidOperationException();

        bikeLocationTracking.IsActive = false;
        bikeLocationTracking.UpdatedOn = DateTime.UtcNow;
        bikeLocationTracking.Address = address;
        bikeLocationTracking.Latitude = bikeCheckoutDto.Latitude;
        bikeLocationTracking.Longitude = bikeCheckoutDto.Longitude;
        
        await _unitOfWork.BikeLocationTrackingHistoryRepository.Add(new BikeLocationTrackingHistory
        {
            BikeId = bikeCheckoutDto.BikeId,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            IsActive = true,
            Latitude = bikeCheckoutDto.Latitude,
            Longitude = bikeCheckoutDto.Longitude,
            Address = address,
            BikeRentalTracking = finishBikeRentalTracking
        });
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
        bikeRentalTracking.TotalPoint = 20;

        return bikeRentalTracking;
    }
    
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
    
    private async Task UpdateBikeRentalTracking(BikeLocationDto bikeLocationDto, string address)
    {
        var bikeRentalTracking = (await _unitOfWork.BikeLocationTrackingRepository.Find(
            b => b.BikeId == bikeLocationDto.BikeId)).FirstOrDefault();
        
        bikeRentalTracking!.Latitude = bikeLocationDto.Latitude;
        bikeRentalTracking.Longitude = bikeLocationDto.Longitude;
        bikeRentalTracking.UpdatedOn = DateTime.UtcNow;
        bikeRentalTracking.Address = address;

        var latestBikeRentalHistory = (await _unitOfWork.BikeLocationTrackingHistoryRepository.Find(
            x => x.BikeId == bikeLocationDto.BikeId)).FirstOrDefault();
        
        await _unitOfWork.BikeLocationTrackingHistoryRepository.Add(new BikeLocationTrackingHistory
        {
            BikeId = bikeLocationDto.BikeId,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            IsActive = true,
            Latitude = bikeLocationDto.Latitude,
            Longitude = bikeLocationDto.Longitude,
            Address = address,
            BikeRentalTrackingId = latestBikeRentalHistory!.BikeRentalTrackingId
        });
    }
}
