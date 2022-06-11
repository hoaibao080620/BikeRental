using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;

namespace BikeService.Sonic.Models;

public class BikeStationManager : BaseEntity
{
    public int ManagerId { get; set; }
    [ForeignKey("ManagerId")]
    public Manager Manager { get; set; } = null!;
    public int BikeStationId { get; set; }
    [ForeignKey("BikeStationId")]
    public BikeStation BikeStation { get; set; } = null!;
}