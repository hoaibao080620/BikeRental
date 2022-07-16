﻿using BikeBookingService.Dtos;
using BikeBookingService.Dtos.BikeOperation;
using BikeBookingService.Dtos.History;
using BikeService.Sonic.Dtos.BikeOperation;

namespace BikeBookingService.BLL;

public interface IBikeTrackingBusinessLogic
{
    Task<List<BikeRentingHistory>> GetBikeRentingHistories(string email);
    Task<List<BikeTrackingRetrieveDto>> GetBikesTracking(string email);
    Task BikeChecking(BikeCheckinDto bikeCheckinDto, string accountEmail);
    Task BikeCheckout(BikeCheckoutDto bikeCheckingDto, string accountEmail);
    Task UpdateBikeLocation(BikeLocationDto bikeLocationDto);
    Task<BikeRentingStatus> GetBikeRentingStatus(string accountEmail);
}