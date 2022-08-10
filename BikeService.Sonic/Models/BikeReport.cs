using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;

namespace BikeService.Sonic.Models;

public class BikeReport : BaseEntity
{
    public int BikeId { get; set; }
    [ForeignKey(nameof(BikeId))]
    public Bike Bike { get; set; } = null!;
    public int? AssignToId { get; set; }
    [ForeignKey(nameof(AssignToId))]
    public Manager? AssignTo { get; set; } = null!;
    public DateTime? CompletedOn { get; set; }
    public string ReportDescription { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string AccountPhoneNumber { get; set; } = null!;
    public string AccountEmail { get; set; } = null!;
    public string Title { get; set; } = null!;
}
