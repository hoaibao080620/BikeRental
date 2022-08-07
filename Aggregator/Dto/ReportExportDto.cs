namespace Aggregator.Dto;

public class ReportExportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalTransaction { get; set; }
    public string ReportType { get; set; } = null!;
    public double Revenue { get; set; }
    public int TotalAccount { get; set; }
    public int TotalBooking { get; set; }
    public int TotalBikeReport { get; set; }
    public List<int> ChartData { get; set; } = null!;
    public List<string> ChartColumns { get; set; } = null!;
}
