namespace BikeBookingService.Validations;

public interface IBikeTrackingValidation
{
    ValueTask<bool> IsBikeCheckinWrongTime(DateTime checkinTime);
    ValueTask<bool> IsAccountHasEnoughPoint(string accountEmail);
}
