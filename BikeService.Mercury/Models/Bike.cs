using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;

namespace BikeService.Mercury.Models;

public class Bike : BaseEntity
{
    [Required]
    public string LicensePlate { get; set; } = null!;
    [Required]
    public int CategoryId { get; set; }
    [ForeignKey("CategoryId")]
    public BikeCategory Category { get; set; } = null!;
    
    public int? BikeStation { get; set; }
    public BikeStation? Station { get; set; }
}