namespace BikeRental.MessageQueue.Events;

public class BikeUpdated : BaseMessage
{
    public int Id { get; set; }
    public string? LicensePlate { get; set; }
    public string? Description { get; set; }
    public int? BikeStationId { get; set; }
    public string? BikeStationName { get; set; }
    public string? BikeStationCode { get; set; }
    public string? Color { get; set; }
    public string? ManagerEmail { get; set; }
}
