﻿using System.ComponentModel.DataAnnotations.Schema;
using BikeTrackingService.Models;
using Shared.Models;

namespace BikeService.Sonic.Models;

public class BikeLocationTrackingHistory : BaseEntity
{
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public string Address { get; set; } = null!;
    public int BikeId { get; set; }
    [ForeignKey("BikeId")]
    public Bike Bike { get; set; } = null!;
    public int BikeRentalTrackingId { get; set; }
    [ForeignKey("BikeRentalTrackingId")]
    public BikeRentalTracking BikeRentalTracking { get; set; } = null!;
}
