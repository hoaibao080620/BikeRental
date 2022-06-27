﻿namespace BikeService.Sonic.DAL;

public interface IUnitOfWork
{
    IBikeRepository BikeRepository { get; }
    IBikeLocationTrackingRepository BikeLocationTrackingRepository { get; }
    IAccountRepository AccountRepository { get; }
    IBikeRentalTrackingHistoryRepository BikeRentalTrackingHistoryRepository { get; }
    IBikeRentalBookingRepository BikeRentalBookingRepository { get; }
    IBikeStationRepository BikeStationRepository { get; }
    IBikeStationManagerRepository BikeStationManagerRepository { get; }
    Task<int> SaveChangesAsync();
}