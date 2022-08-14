namespace BikeRental.MessageQueue.Events;

public class UserDeleted : BaseMessage
{
    public string UserId { get; set; } = null!;
}
