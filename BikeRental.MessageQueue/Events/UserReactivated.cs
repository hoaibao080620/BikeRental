namespace BikeRental.MessageQueue.Events;

public class UserReactivated : BaseMessage
{
    public string UserId { get; set; } = null!;
}
