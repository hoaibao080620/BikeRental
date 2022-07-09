﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Models;

namespace BikeTrackingService.Models;

public class Bike : BaseEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public new int Id { get; set; }
    public string LicensePlate { get; set; } = null!;
    public string? Description { get; set; }
    public int? BikeStationId { get; set; }
    public string? BikeStationName { get; set; }
    public string? Color { get; set; }
    public string Status { get; set; } = null!;
    public List<BikeLocationTracking> BikeLocationTrackings { get; set; } = null!; 
}
