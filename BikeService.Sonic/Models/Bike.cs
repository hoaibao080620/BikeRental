﻿using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;

namespace BikeService.Sonic.Models;

public class Bike : BaseEntity
{
    public string BikeCode { get; set; } = null!;
    public string? Description { get; set; }
    public int? BikeStationId { get; set; }
    [ForeignKey("BikeStationId")]
    public BikeStation? BikeStation { get; set; }
    public string Status { get; set; } = null!;
    public bool IsLock { get; set; }
}
