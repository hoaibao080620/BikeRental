namespace BikeBookingService.Validations;

public class BikeTrackingValidation : IBikeTrackingValidation
{
    public ValueTask<bool> IsBikeCheckinWrongTime(DateTime checkinTime)
    {
        return new ValueTask<bool>(checkinTime.Hour is >= 22 or < 6);
    }

    public ValueTask<bool> IsAccountHasEnoughPoint(string accountEmail)
    {
        throw new NotImplementedException();
    }
}
