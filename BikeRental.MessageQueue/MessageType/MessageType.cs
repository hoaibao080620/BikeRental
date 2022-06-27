namespace BikeRental.MessageQueue.MessageType;

public class MessageType
{
    public const string UserAdded = "UserAdded";
    public const string UserUpdated = "UserUpdated";
    public const string UserDeleted = "UserDeleted";
    public const string NotifyBikeLocationChange = "NotifyBikeLocationChange";
    public const string NotifyBikeCheckin = "NotifyBikeCheckin";
    public const string NotifyBikeCheckout = "NotifyBikeCheckout";
    public const string PaymentSucceeded = "PaymentSucceeded";
    public const string PaymentFailed = "PaymentFailed";
}
