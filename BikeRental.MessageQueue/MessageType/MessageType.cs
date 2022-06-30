namespace BikeRental.MessageQueue.MessageType;

public class MessageType
{
    public const string UserAdded = "UserAdded";
    public const string UserUpdated = "UserUpdated";
    public const string UserDeleted = "UserDeleted";
    public const string NotifyBikeLocationChange = "NotifyBikeLocationChange";
    public const string BikeCheckedIn = "BikeCheckedIn";
    public const string BikeCheckedOut = "BikeCheckedOut";
    public const string BikeLocationChanged = "BikeLocationChanged";
    public const string PaymentSucceeded = "PaymentSucceeded";
    public const string PaymentFailed = "PaymentFailed";
}
