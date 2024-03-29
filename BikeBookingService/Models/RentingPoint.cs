﻿using System.ComponentModel.DataAnnotations;

namespace BikeBookingService.Models;

public class RentingPoint
{
    [Key]
    public int Id { get; set; }
    public double PointPerHour { get; set; }
    public DateTime CreatedOn { get; set; }
}
