﻿using BikeService.Sonic.Dtos.GoogleMapAPI;

namespace BikeService.Sonic.Services.Interfaces;

public interface IGoogleMapService
{
    Task<string?> GetAddressOfLocation(double longitude, double latitude);
    Task<(double, double)> GetLocationOfAddress(string placeId);
    Task<double> GetDistanceBetweenTwoLocations(GoogleMapLocation origin, GoogleMapLocation destination);
}
