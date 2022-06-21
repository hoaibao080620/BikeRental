using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;

namespace BikeService.Sonic.Models;

public class BikeRentalOrder : BaseEntity
{
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public DateTime CheckinTime { get; set; }
    public DateTime CheckoutTime { get; set; }
    public int BikeId { get; set; }
    [ForeignKey("BikeId")]
    public Bike Bike { get; set; } = null!;
    [ForeignKey("AccountId")]
    public Account Account { get; set; } = null!;
    public int AccountId { get; set; }
    public double Total { get; set; }
}
