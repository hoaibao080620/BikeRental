using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;

namespace BikeService.Sonic.Models;

public class BikeRentalTrackingHistory : BaseEntity
{
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public int BikeId { get; set; }
    [ForeignKey("BikeId")]
    public Bike Bike { get; set; } = null!;
}
