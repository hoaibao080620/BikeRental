namespace BikeService.Sonic.Dtos.Bike;

public class BikeReportRetriveDto
{
    public int Id { get; set; }
    public int BikeId { get; set; }
    public string BikeLicensePlate { get; set; } = null!;
    public string ReportDescription { get; set; } = null!;
    public DateTime ReportOn { get; set; }
    public string Status { get; set; } = null!;
    public string AccountReport { get; set; } = null!;
    public DateTime? CompletedOn { get; set; }
    public string? CompletedBy { get; set; }
}
