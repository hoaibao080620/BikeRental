namespace BikeRental.MessageQueue.Events;

public class BikeLocationChanged : BaseMessage
{
    public int BikeId { get; set; }
    public string LastAddress { get; set; } = null!;
    public double LastLongitude { get; set; }
    public double LastLatitude { get; set; }
}
