using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;

namespace BikeBookingService.Models;

public class BikeRentalBooking : BaseEntity
{
    public DateTime CheckinOn { get; set; }
    public DateTime? CheckoutOn { get; set; }
    public double TotalPoint { get; set; }
    public string CheckinBikeStation { get; set; } = null!;
    public string? CheckoutBikeStation { get; set; }
    public int AccountId { get; set; }
    [ForeignKey(nameof(AccountId))]
    public Account Account { get; set; } = null!;
    public int BikeId { get; set; }
    [ForeignKey(nameof(BikeId))]
    public Bike Bike { get; set; } = null!;

    public string? PaymentStatus { get; set; }
    public List<BikeLocationTrackingHistory> BikeLocationTrackingHistories { get; set; } = null!;
}
