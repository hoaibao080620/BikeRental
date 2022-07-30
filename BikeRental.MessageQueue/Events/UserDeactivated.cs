namespace BikeRental.MessageQueue.Events;

public class UserDeactivated : BaseMessage
{
    public string UserId { get; set; } = null!;
}
