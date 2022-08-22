namespace BikeBookingService.Validations;

public interface IBikeTrackingValidation
{
    ValueTask<bool> IsBikeCheckinOrCheckoutWrongTime(DateTime checkinTime);
    ValueTask<(bool, double)> IsAccountHasEnoughPoint(string accountEmail);
    ValueTask<bool> IsAccountHasBikeRentingNotFullyPaid(string accountEmail);
    ValueTask<bool> IsAccountHasBikeRentingPending(string accountEmail);
    ValueTask<bool> IsAccountIsRentingBike(string accountEmail);
    ValueTask<bool> IsBikeAlreadyRent(string bikeCode);
    ValueTask<bool> IsBikeStatusAvailable(string bikeCode);
}
