namespace BikeRental.MessageQueue;

public abstract class BaseMessage
{
    public string MessageType { get; set; } = null!;
}