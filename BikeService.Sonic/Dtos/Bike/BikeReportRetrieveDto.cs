namespace BikeService.Sonic.Dtos.Bike;

public class BikeReportRetriveDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string BikeCode { get; set; } = null!;
    public string ReportDescription { get; set; } = null!;
    public DateTime ReportOn { get; set; }
    public string Status { get; set; } = null!;
    public string AccountPhoneNumber { get; set; } = null!;
    public DateTime? CompletedOn { get; set; }
    public string? CompletedBy { get; set; }
    public string? ImageUrl { get; set; }
}
