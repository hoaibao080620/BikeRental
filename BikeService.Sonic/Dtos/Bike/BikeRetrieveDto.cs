namespace BikeService.Sonic.Dtos.Bike;

public class BikeRetrieveDto
{
    public int Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public string LicensePlate { get; set; } = null!;
    public string? Description { get; set; }
    public int? BikeStationId { get; set; }
    public string Status { get; set; } = null!;
    public string? BikeStationName { get; set; }
}
