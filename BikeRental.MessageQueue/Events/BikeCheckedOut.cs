namespace BikeRental.MessageQueue.Events;

public class BikeCheckedOut : BaseMessage
{
    public List<string> ManagerEmails { get; set; }
    public string AccountEmail { get; set; } = null!;
    public int BikeId { get; set; }
    public string LicensePlate { get; set; } = null!;
    public string BikeStationName { get; set; } = null!;
    public string BikeCode { get; set; }
    public DateTime CheckoutOn { get; set; }
    public double RentingPoint { get; set; }
}
