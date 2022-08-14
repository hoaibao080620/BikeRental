using BikeBookingService.Const;
using BikeBookingService.DAL;
using BikeBookingService.Dtos;
using BikeBookingService.Dtos.BikeOperation;
using BikeBookingService.Dtos.History;
using BikeBookingService.MessageQueue.Publisher;
using BikeBookingService.Models;
using BikeBookingService.Services;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.MessageType;
using Grpc.Net.ClientFactory;
using Microsoft.EntityFrameworkCore;

namespace BikeBookingService.BLL;

public class BikeTrackingBusinessLogic : IBikeTrackingBusinessLogic
{
    private readonly BikeServiceGrpc.BikeServiceGrpcClient _bikeServiceGrpc;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGoogleMapService _googleMapService;
    private readonly IMessageQueuePublisher _messageQueuePublisher;
    private readonly AccountServiceGrpc.AccountServiceGrpcClient _accountServiceGrpc;

    public BikeTrackingBusinessLogic(
        GrpcClientFactory grpcClientFactory,
        IUnitOfWork unitOfWork,
        IGoogleMapService googleMapService,
        IMessageQueuePublisher messageQueuePublisher)
    {
        _bikeServiceGrpc = grpcClientFactory.CreateClient<BikeServiceGrpc.BikeServiceGrpcClient>("BikeService");
        _accountServiceGrpc = grpcClientFactory.CreateClient<AccountServiceGrpc.AccountServiceGrpcClient>("AccountService");
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
                BikePlate = x.Bike.BikeCode,
                AccountEmail = x.Account.Email,
                CheckedInOn = x.CheckinOn,
                CheckedOutOn = x.CheckoutOn,
                AccountPhone = x.Account.PhoneNumber,
                TotalTime = x.CheckoutOn.HasValue ? x.CheckoutOn.Value.Subtract(x.CheckinOn).TotalMinutes : null,
                TotalPoint = x.TotalPoint,
                PaymentStatus = x.PaymentStatus,
                Status = x.CheckoutOn.HasValue ? "Done" : "InProgress",
                CheckInStation = x.CheckinBikeStation,
                CheckOutStation = x.CheckoutBikeStation
            }).OrderByDescending(x => x.CheckedInOn).ToList();

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
                LicensePlate = x.BikeCode,
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
        var bike = await GetBikeByCode(bikeCheckinDto.BikeCode);
        var managerEmails = (await _bikeServiceGrpc.GetManagerEmailsOfBikeIdAsync(new GetManagerEmailsRequest
        {
            BikeId = bike.Id
        })).ManagerEmails.ToList();
        
        var address = await _googleMapService.GetAddressOfLocation(
            bikeCheckinDto.Longitude,
            bikeCheckinDto.Latitude);
        address ??= "N/A";
        
        var pushEventToMapTask = _messageQueuePublisher.PublishBikeLocationChangeCommand(managerEmails);
        var pushNotificationToManagers = _messageQueuePublisher.PublishBikeCheckedInEvent(
            new BikeCheckedIn
            {
                ManagerEmails = managerEmails,
                BikeId = bike.Id,
                BikeStationId = bike.BikeStationId!.Value,
                BikeStationName = bike.BikeStationName!,
                AccountEmail = accountEmail,
                LicensePlate = bike.BikeCode,
                CheckinOn = bikeCheckinDto.CheckinTime,
                MessageType = MessageType.BikeCheckedIn
            });
        
        var startTrackingBikeTask = StartTrackingBike(bikeCheckinDto, address, accountEmail, bike.Id);
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
        var bikeRenting = (await _unitOfWork.BikeRentalTrackingRepository
            .Find(x => x.Account.Email == accountEmail && x.CheckoutOn == null)).FirstOrDefault()!;
        
        var managerEmails = (await _bikeServiceGrpc.GetManagerEmailsOfBikeIdAsync(new GetManagerEmailsRequest
        {
            BikeId = bikeRenting.BikeId
        })).ManagerEmails.ToList();
        
        var bike = await GetBikeById(bikeRenting.BikeId);
        var address = await _googleMapService.GetAddressOfLocation(
            bikeCheckout.Longitude,
            bikeCheckout.Latitude);

        address ??= "N/A";
        var rentingPoint = GetRentingPoint(bikeRenting.CheckinOn, bikeCheckout.CheckoutOn);
        var pushEventToMapTask = _messageQueuePublisher.PublishBikeLocationChangeCommand(managerEmails);
        var stopTrackingBikeTask = StopTrackingBike(new StopTrackingBikeParam
        {
            AccountEmail = accountEmail,
            Address = address,
            BikeCheckoutDto = bikeCheckout,
            BikeId = bikeRenting.BikeId,
            RentingPoint = rentingPoint
        });
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
                BikeCode = bikeCheckout.Code,
                AccountEmail = accountEmail,
                LicensePlate = bike.BikeCode,
                CheckoutOn = bikeCheckout.CheckoutOn,
                RentingPoint = rentingPoint,
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
        var bike = await GetBikeByCode(bikeLocationDto.BikeCode);
        var managerEmails = (await _bikeServiceGrpc.GetManagerEmailsOfBikeIdAsync(new GetManagerEmailsRequest
        {
            BikeId = bike.Id
        })).ManagerEmails.ToList();
        
        var address = await _googleMapService.GetAddressOfLocation(
            bikeLocationDto.Longitude, 
            bikeLocationDto.Latitude);
        address ??= "N/A";

        var updateBikeTracking = UpdateBikeRentalTracking(bikeLocationDto, address, bike.Id);
        
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
            .Find(b => !b.CheckoutOn.HasValue && b.Account.Email == accountEmail);
    
        return rentingStatus.Any()
            ? rentingStatus.Select(x => new BikeRentingStatus
            {
                AccountEmail = accountEmail,
                IsRenting = true,
                BikeId = x.BikeId,
                LicensePlate = x.Bike.BikeCode,
                TimeUsing = DateTime.UtcNow.Subtract(x.CheckinOn).Milliseconds,
                Cost = Math.Round(GetRentingPoint(x.CheckinOn, DateTime.UtcNow), 2)
            }).FirstOrDefault()!
            : new BikeRentingStatus
            {
                AccountEmail = accountEmail,
                IsRenting = false,
                BikeId = null
            };
    }

    public async Task<List<BikeBookingHistoryDto>> GetBikeBookingHistories(string accountEmail)
    {
        var bikeBookings = (await _unitOfWork.BikeRentalTrackingRepository
                .Find(x => x.Account.Email == accountEmail && x.CheckoutOn.HasValue))
            .Select(x => new BikeBookingHistoryDto
            {
                CheckinOn = x.CheckinOn,
                CheckoutOn = x.CheckoutOn!.Value,
                AccountPhoneNumber = x.Account.PhoneNumber,
                BikeLicensePlate = x.Bike.BikeCode,
                CheckinBikeStation = x.CheckinBikeStation,
                CheckoutBikeStation = x.CheckoutBikeStation!,
                PaymentStatus = x.PaymentStatus!,
                TotalPoint = x.TotalPoint,
                TotalDistance = x.BikeLocationTrackingHistories.Sum(xx => xx.DistanceFromPreviousLocation)
            });

        return bikeBookings.ToList();
    }

    public async Task CheckBikeRentingHasUserAlmostRunOutPoint()
    {
        var bikeRentalBookings = (await _unitOfWork.BikeRentalTrackingRepository
                .Find(x => x.CheckoutOn == null))
            .Include(x => x.Account);
        var rentingPoint = (await _unitOfWork.RentingPointRepository.Find(_ => true)).First();
        var rentingPointWarning = rentingPoint.PointPerHour / 2;

        foreach (var bikeRentalBooking in bikeRentalBookings)
        {
            var email = bikeRentalBooking.Account.Email;
            var currentRentingPoint = GetRentingPoint(bikeRentalBooking.CheckinOn, DateTime.UtcNow);
            var accountPoint = (await _accountServiceGrpc.GetAccountInfoAsync(new GetAccountInfoRequest
            {
                Email = email
            })).Point;

            if (accountPoint - currentRentingPoint <= rentingPointWarning)
            {
                await _messageQueuePublisher.PublishUserPointRunOutEvent(new UserAlmostRunOutPoint
                {
                    Email = email,
                    Message = "Tài khoản của bạn sắp hết điểm, vui lòng trả xe hoặc nạp thêm điểm!",
                    MessageType = MessageType.UserAlmostRunOutPoint
                });
                bikeRentalBooking.IsWarningNotificationSend = true;
            }
        }
    }

    public async Task TestNotification(string email)
    {
        await _messageQueuePublisher.PublishUserPointRunOutEvent(new UserAlmostRunOutPoint
        {
            Email = email,
            Message = "Tài khoản của bạn sắp hết điểm, vui lòng trả xe hoặc nạp thêm điểm!",
            MessageType = MessageType.UserAlmostRunOutPoint
        });
    }

    private async Task<Bike> GetBikeById(int bikeId)
    {
        var bike = await _unitOfWork.BikeRepository.GetById(bikeId) ?? throw new InvalidOperationException();
        return bike;
    }
    
    private async Task<Bike> GetBikeByCode(string code)
    {
        var bike = await _unitOfWork.BikeRepository.Find(x => x.BikeCode == code);
        return bike.FirstOrDefault() ?? throw new InvalidOperationException();;
    }
    
    private async Task StartTrackingBike(BikeCheckinDto bikeCheckinDto, string address, string accountEmail, int bikeId)
    {
        var bikeRentalTracking = await CreateBikeRentalBooking(
            bikeCheckinDto, accountEmail);

        var bikeTracking = (await _unitOfWork.BikeLocationTrackingRepository.Find(b =>
            b.BikeId == bikeId&& b.IsActive == false)).FirstOrDefault();

        if (bikeTracking is null)
        {
            await _unitOfWork.BikeLocationTrackingRepository.Add(new BikeLocationTracking
            {
                BikeId = bikeId,
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
            BikeId = bikeId,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            IsActive = true,
            Latitude = bikeCheckinDto.Latitude,
            Longitude = bikeCheckinDto.Longitude,
            Address = address,
            BikeRentalBooking = bikeRentalTracking
        });
    }
    
    private async Task<BikeRentalBooking> CreateBikeRentalBooking(BikeCheckinDto bikeCheckinDto, string accountEmail)
    {
        var bike = await GetBikeByCode(bikeCheckinDto.BikeCode);
        var account = await GetAccountByEmail(accountEmail);
        var bikeRentalTracking = new BikeRentalBooking
        {
            CheckinOn = bikeCheckinDto.CheckinTime,
            AccountId = account.Id,
            BikeId = bike.Id,
            IsActive = true,
            CreatedOn = DateTime.UtcNow,
            CheckinBikeStation = bike.BikeStationName!
        };
        
        await _unitOfWork.BikeRentalTrackingRepository.Add(bikeRentalTracking);
        return bikeRentalTracking;
    }
    
    private async Task<Account> GetAccountByEmail(string email)
    {
        return (await _unitOfWork.AccountRepository.Find(a => a.Email == email)).FirstOrDefault() 
               ?? throw new InvalidOperationException($"Account with email {email} not found!");
    }
    
    private async Task StopTrackingBike(StopTrackingBikeParam stopTrackingBikeParam)
    {
        var finishBikeRentalBooking = await FinishBikeRentalBooking(
            stopTrackingBikeParam.BikeCheckoutDto, stopTrackingBikeParam.AccountEmail, stopTrackingBikeParam.RentingPoint); 
        var bikeLocationTracking = (await _unitOfWork.BikeLocationTrackingRepository
                                       .Find(b => b.BikeId == stopTrackingBikeParam.BikeId)).FirstOrDefault()
                                   ?? throw new InvalidOperationException();

        bikeLocationTracking.IsActive = false;
        bikeLocationTracking.UpdatedOn = DateTime.UtcNow;
        bikeLocationTracking.Address = stopTrackingBikeParam.Address;
        bikeLocationTracking.Latitude = stopTrackingBikeParam.BikeCheckoutDto.Latitude;
        bikeLocationTracking.Longitude = stopTrackingBikeParam.BikeCheckoutDto.Longitude;
        
        await _unitOfWork.BikeLocationTrackingHistoryRepository.Add(new BikeLocationTrackingHistory
        {
            BikeId = stopTrackingBikeParam.BikeId,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            IsActive = true,
            Latitude = stopTrackingBikeParam.BikeCheckoutDto.Latitude,
            Longitude = stopTrackingBikeParam.BikeCheckoutDto.Longitude,
            Address = stopTrackingBikeParam.Address,
            BikeRentalBooking = finishBikeRentalBooking,
            DistanceFromPreviousLocation = 10
        });
    }
    
    private async Task<BikeRentalBooking> FinishBikeRentalBooking(BikeCheckoutDto bikeCheckoutDto, string accountEmail, double rentingPoint)
    {
        var bikeRentalBooking = (await _unitOfWork.BikeRentalTrackingRepository.Find(b =>
                !b.CheckoutOn.HasValue && b.Account.Email == accountEmail))
            .OrderByDescending(b => b.CreatedOn).
            FirstOrDefault();
        
        ArgumentNullException.ThrowIfNull(bikeRentalBooking);
        var bike = await GetBikeById(bikeRentalBooking.BikeId);
        
        bikeRentalBooking.CheckoutOn = bikeCheckoutDto.CheckoutOn;
        bikeRentalBooking.UpdatedOn = DateTime.UtcNow;
        bikeRentalBooking.TotalPoint = rentingPoint;
        bikeRentalBooking.PaymentStatus = PaymentStatus.PENDING;

        if (bikeCheckoutDto.Code != bike.BikeCode)
        {
            var bikeStationName = (await _bikeServiceGrpc.GetBikeStationByCodeOrIdAsync(new GetBikeStationByCodeOrIdRequest
            {
                Code = bikeCheckoutDto.Code
            })).Name;

            bikeRentalBooking.CheckoutBikeStation = bikeStationName;
        }
        else
        {
            bikeRentalBooking.CheckoutBikeStation = bikeRentalBooking.CheckinBikeStation;
        }

        return bikeRentalBooking;
    }
    
    private static double GetRentingPoint(DateTime checkinOn, DateTime checkoutOn)
    {
        var duration = checkoutOn.Subtract(checkinOn).TotalHours;
        return Math.Round(duration * 20, 2, MidpointRounding.ToZero);
    }
    
    private async Task UpdateBikeRentalTracking(BikeLocationDto bikeLocationDto, string address, int bikeId)
    {
        var bikeRentalTracking = (await _unitOfWork.BikeLocationTrackingRepository.Find(
            b => b.BikeId == bikeId)).FirstOrDefault();
        
        bikeRentalTracking!.Latitude = bikeLocationDto.Latitude;
        bikeRentalTracking.Longitude = bikeLocationDto.Longitude;
        bikeRentalTracking.UpdatedOn = DateTime.UtcNow;
        bikeRentalTracking.Address = address;

        var latestBikeRentalHistory = (await _unitOfWork.BikeLocationTrackingHistoryRepository.Find(
            x => x.BikeId == bikeId)).FirstOrDefault();
        
        await _unitOfWork.BikeLocationTrackingHistoryRepository.Add(new BikeLocationTrackingHistory
        {
            BikeId = bikeId,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
            IsActive = true,
            Latitude = bikeLocationDto.Latitude,
            Longitude = bikeLocationDto.Longitude,
            Address = address,
            BikeRentalTrackingId = latestBikeRentalHistory!.BikeRentalTrackingId,
            DistanceFromPreviousLocation = bikeLocationDto.Distance
        });
    }
    
}
