﻿namespace BikeService.Sonic.Dtos;

public class BikeCheckingDto
{
    public int BikeId { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public string UserEmail { get; set; } = null!;
}