namespace BikeRental.MessageQueue.Events;

public class BikeStationColorUpdated : BaseMessage
{
    public List<BikeStationColor> BikeStationColors { get; set; } = null!;
    public List<string> ManagerEmails { get; set; } = null!;
}

public class BikeStationColor
{
    public int BikeStationId { get; set; }
    public string? Color { get; set; }
}
