using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;

namespace BikeService.Sonic.Models;

public class BikeLocationTracking : BaseEntity
{
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public string Address { get; set; } = null!;
    public int BikeId { get; set; }
    [ForeignKey("BikeId")]
    public Bike Bike { get; set; } = null!;
}
