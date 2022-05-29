using System.ComponentModel.DataAnnotations;
using Shared.Models;

namespace BikeService.Mercury.Models;

public class Bike : BaseEntity
{
    [Required]
    public string LicensePlate { get; set; } = null!;
    public int? BikeStation { get; set; }
    public BikeStation? Station { get; set; }
}
