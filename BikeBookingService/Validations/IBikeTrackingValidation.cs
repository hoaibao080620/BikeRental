namespace BikeBookingService.Validations;

public interface IBikeTrackingValidation
{
    ValueTask<bool> IsBikeCheckinOrCheckoutWrongTime(DateTime checkinTime);
    ValueTask<bool> IsAccountHasEnoughPoint(string accountEmail, string token);
    ValueTask<bool> IsAccountHasBikeRentingNotFullyPaid(string accountEmail);
    ValueTask<bool> IsAccountHasBikeRentingPending(string accountEmail);
    ValueTask<bool> IsAccountIsRentingBike(string accountEmail);
}
