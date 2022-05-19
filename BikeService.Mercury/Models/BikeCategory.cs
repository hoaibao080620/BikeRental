using System.ComponentModel.DataAnnotations;
using Shared.Models;

namespace BikeService.Mercury.Models;

public class BikeCategory : BaseEntity
{
    [Required]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}