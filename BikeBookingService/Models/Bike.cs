using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;

namespace BikeBookingService.Models;

public class Bike : BaseEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public new int Id { get; set; }
    public string LicensePlate { get; set; } = null!;
    public string? Description { get; set; }
    public int? BikeStationId { get; set; }
    public string? BikeStationName { get; set; }
    public string? Color { get; set; }
    public List<BikeLocationTracking> BikeLocationTrackings { get; set; } = null!; 
}
