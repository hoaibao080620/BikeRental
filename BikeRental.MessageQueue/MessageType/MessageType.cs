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
    public const string BikeCreated = "BikeCreated";
    public const string BikeUpdated = "BikeUpdated";
    public const string BikeDeleted = "BikeDeleted";
    public const string BikeStationColorUpdated = "BikeStationColorUpdated";
    public const string UserRoleUpdated = "UserRoleUpdated";
    public const string AccountPointSubtracted = "AccountPointSubtracted";
    public const string AccountPointLimitExceeded = "AccountPointLimitExceeded";
    public const string AccountDebtHasBeenPaid = "AccountDebtHasBeenPaid";
    public const string AccountDeactivated = "AccountDeactivated";
    public const string AccountReactivated = "AccountReactivated";
    public const string UserAlmostRunOutPoint = "UserAlmostRunOutPoint";
}
