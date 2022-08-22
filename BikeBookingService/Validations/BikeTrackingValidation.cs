using System.Net.Http.Headers;
using BikeBookingService.Const;
using BikeBookingService.DAL;
using Grpc.Net.ClientFactory;

namespace BikeBookingService.Validations;

public class BikeTrackingValidation : IBikeTrackingValidation
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AccountServiceGrpc.AccountServiceGrpcClient _accountService;
    private readonly BikeServiceGrpc.BikeServiceGrpcClient _bikeService;

    public BikeTrackingValidation(IUnitOfWork unitOfWork, GrpcClientFactory grpcClientFactory)
    {
        _unitOfWork = unitOfWork;
        _accountService = grpcClientFactory.CreateClient<AccountServiceGrpc.AccountServiceGrpcClient>("AccountService");
        _bikeService = grpcClientFactory.CreateClient<BikeServiceGrpc.BikeServiceGrpcClient>("BikeService");
    }
    
    public ValueTask<bool> IsBikeCheckinOrCheckoutWrongTime(DateTime checkinTime)
    {
        var localTime = TimeZoneInfo.ConvertTimeFromUtc(checkinTime, TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok"));
        return new ValueTask<bool>(localTime.Hour is >= 22 or < 6);
    }

    public async ValueTask<(bool, double)> IsAccountHasEnoughPoint(string accountEmail)
    {
        var currentRentingPoint = (await _unitOfWork.RentingPointRepository.All()).FirstOrDefault();
        var accountInfo = await _accountService.GetAccountInfoAsync(new GetAccountInfoRequest
        {
            Email = accountEmail
        });

        return (accountInfo.Point >= currentRentingPoint?.PointPerHour, currentRentingPoint.PointPerHour);
    }

    public async ValueTask<bool> IsAccountHasBikeRentingNotFullyPaid(string accountEmail)
    {
        var isAccountHasBikeRentingNotFullyPaid = await _unitOfWork.BikeRentalTrackingRepository.Exists(x =>
            x.Account.Email == accountEmail && 
            x.PaymentStatus == PaymentStatus.NOT_FULLY_PAID);

        return isAccountHasBikeRentingNotFullyPaid;
    }

    public async ValueTask<bool> IsAccountHasBikeRentingPending(string accountEmail)
    {
        var isAccountHasBikeRentingNotFullyPaid = await _unitOfWork.BikeRentalTrackingRepository.Exists(x =>
            x.Account.Email == accountEmail &&
            x.PaymentStatus == PaymentStatus.PENDING);

        return isAccountHasBikeRentingNotFullyPaid;
    }

    public async ValueTask<bool> IsAccountIsRentingBike(string accountEmail)
    {
        var isAccountHasRentingBike = (await _unitOfWork.BikeRentalTrackingRepository
                .Find(x => x.Account.Email == accountEmail && x.CheckoutOn == null))
            .Any();

        return isAccountHasRentingBike;
    }

    public async ValueTask<bool> IsBikeAlreadyRent(string bikeCode)
    {
        var isBikeHasBeenRenting = await _unitOfWork.BikeRentalTrackingRepository
            .Find(x => x.Bike.BikeCode == bikeCode && x.CheckoutOn == null);

        return isBikeHasBeenRenting.Any();
    }

    public async ValueTask<bool> IsBikeStatusAvailable(string bikeCode)
    {
        var bike = (await _unitOfWork.BikeRepository.Find(x => x.BikeCode == bikeCode)).FirstOrDefault();
        var status = await _bikeService.GetBikeStatusAsync(new GetBikeStatusRequest
        {
            BikeId = bike!.Id
        });

        return status.Status == "Available";
    }
}
