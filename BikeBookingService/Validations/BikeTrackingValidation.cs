using System.Net.Http.Headers;
using BikeBookingService.Const;
using BikeBookingService.DAL;

namespace BikeBookingService.Validations;

public class BikeTrackingValidation : IBikeTrackingValidation
{
    private readonly IUnitOfWork _unitOfWork;

    public BikeTrackingValidation(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public ValueTask<bool> IsBikeCheckinOrCheckoutWrongTime(DateTime checkinTime)
    {
        return new ValueTask<bool>(checkinTime.Hour is >= 22 or < 6);
    }

    public async ValueTask<bool> IsAccountHasEnoughPoint(string accountEmail, string token)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", 
            token.Replace("Bearer", string.Empty));
        var result = await httpClient.GetStringAsync(
            "https://bike-rental-account-service.herokuapp.com/account/" +
            $"isAccountHasEnoughPoint?accountEmail={accountEmail}");

        return result == "true";
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
}
