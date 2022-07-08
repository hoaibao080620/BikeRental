using Shared.Models;

namespace BikeTrackingService.Models;

public class Account : BaseEntity
{
    public string Email { get; set; } = null!;
    public double Point { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string ExternalId { get; set; } = null!;
}
