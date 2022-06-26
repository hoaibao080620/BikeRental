namespace BikeRental.MessageQueue.Commands;

public class PushBikeCheckoutNotification : BaseMessage
{
    public string AccountEmail { get; set; } = null!;
    public List<string> ManagerEmails { get; set; } = null!;
    public int BikeId { get; set; }
    public int BikeStationId { get; set; }
    public string LicensePlate { get; set; } = null!;
    public string BikeStationName { get; set; } = null!;
    public DateTime CheckoutOn { get; set; }
}
