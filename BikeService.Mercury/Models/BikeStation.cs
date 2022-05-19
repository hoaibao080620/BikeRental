using System.ComponentModel.DataAnnotations;
using Shared.Models;

namespace BikeService.Mercury.Models;

public class BikeStation : BaseEntity
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Address { get; set; } = null!;

    [Required]
    public double Longitude { get; set; }
    
    [Required]
    public double Latitude { get; set; }

    public string? Description { get; set; }
    public int ParkingSpace { get; set; }
    public int UsedParkingSpace { get; set; }
}