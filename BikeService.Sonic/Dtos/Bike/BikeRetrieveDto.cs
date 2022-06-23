namespace BikeService.Sonic.Dtos.Bike;

public class BikeRetrieveDto
{
    public int Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public bool IsActive { get; set; }
    public string LicensePlate { get; set; } = null!;
    public string? Description { get; set; }
    public int? BikeStationId { get; set; }
    public string Status { get; set; } = null!;
    public string? BikeStationName { get; set; }
    public string? LastAddress { get; set; }
    public double? LastLongitude { get; set; }
    public double? LastLatitude { get; set; }

    public bool IsRenting { get; set; }
    // public DateTime LastCheckin { get; set; }
    // public DateTime LastCheckinUsername { get; set; }
}
