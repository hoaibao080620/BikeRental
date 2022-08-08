using System.ComponentModel.DataAnnotations;

namespace BikeBookingService.Models;

public class RentingPointHistory
{
    [Key]
    public int Id { get; set; }
    public double PointChange { get; set; }
    public string? ChangeReason { get; set; }
    public DateTime CreatedOn { get; set; }
    public string ChangeBy { get; set; } = null!;
}
