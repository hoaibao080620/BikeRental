namespace BikeRental.MessageQueue.Events;

public class BikeCreated : BaseMessage
{
    public int Id { get; set; }
    public string LicensePlate { get; set; } = null!;
    public string? Description { get; set; }
    public int? BikeStationId { get; set; }
    public string? BikeStationName { get; set; }
    public string Status { get; set; } = null!;
    public string? Color { get; set; }
}
