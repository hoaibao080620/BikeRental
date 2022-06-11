using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;

namespace BikeService.Sonic.Models;

public class BikeRentalTracking : BaseEntity
{
    public DateTime StartedOn { get; set; }
    public DateTime? EndedOn { get; set; }
    public string StartLongitude { get; set; } = null!;
    public string StartLatitude { get; set; } = null!;
    public string? EndLongitude { get; set; }
    public string? EndLatitude { get; set; }
    public double? TotalDistance { get; set; }
    public int BikeId { get; set; }
    [ForeignKey("BikeId")]
    public Bike Bike { get; set; } = null!;
    [ForeignKey("AccountId")]
    public Account Account { get; set; } = null!;
    public int AccountId { get; set; }
    public string State { get; set; } = null!;
}