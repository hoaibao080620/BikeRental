using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;

namespace BikeService.Sonic.Models;

public class BikeRentalTracking : BaseEntity
{
    public DateTime StartedOn { get; set; }
    public DateTime? EndedOn { get; set; }
    public double StartLongitude { get; set; }
    public double StartLatitude { get; set; }
    public double? EndLongitude { get; set; }
    public double? EndLatitude { get; set; }
    public int BikeId { get; set; }
    [ForeignKey("BikeId")]
    public Bike Bike { get; set; } = null!;
    [ForeignKey("AccountId")]
    public Account Account { get; set; } = null!;
    public int AccountId { get; set; }
}