namespace BikeService.Sonic.Dtos.Bike;

public class BikeUpdateDto
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public int? BikeStationId { get; set; }
    public string Status { get; set; } = null!;
}
