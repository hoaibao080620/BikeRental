namespace BikeRental.MessageQueue.Events;

public class BikeCheckedIn : BaseMessage
{
    public List<string> ManagerEmails { get; set; }
    public string AccountEmail { get; set; } = null!;
    public int BikeId { get; set; }
    public int BikeStationId { get; set; }
    public string BikeStationName { get; set; } = null!;
    public string LicensePlate { get; set; } = null!;
    public DateTime CheckinOn { get; set; }
}
