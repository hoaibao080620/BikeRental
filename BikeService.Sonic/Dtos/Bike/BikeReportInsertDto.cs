namespace BikeService.Sonic.Dtos.Bike;

public class BikeReportInsertDto
{
    public int BikeId { get; set; }
    public string Title { get; set; } = null!;
    public string ReportDescription { get; set; } = null!;
    public string? ImageBase64 { get; set; }
}
