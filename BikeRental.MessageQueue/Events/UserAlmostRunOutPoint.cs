namespace BikeRental.MessageQueue.Events;

public class UserAlmostRunOutPoint : BaseMessage
{
    public string Email { get; set; } = null!;
    public string Message { get; set; } = null!;
}
