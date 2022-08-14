namespace BikeRental.MessageQueue.Events;

public class BikeDeleted : BaseMessage
{
    public int Id { get; set; }
}
