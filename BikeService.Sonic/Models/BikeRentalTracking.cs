using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;

namespace BikeService.Sonic.Models;

public class BikeRentalTracking : BaseEntity
{
    public DateTime CheckinOn { get; set; }
    public DateTime? CheckoutOn { get; set; }
    public double TotalPoint { get; set; }
    public int AccountId { get; set; }
    [ForeignKey(nameof(AccountId))]
    public Account Account { get; set; } = null!;
    public int BikeId { get; set; }
    [ForeignKey(nameof(BikeId))]
    public Bike Bike { get; set; } = null!;
}
