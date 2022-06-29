﻿using BikeService.Sonic.BikeDbContext;

namespace BikeService.Sonic.DAL;

public class UnitOfWork : IUnitOfWork
{
    private readonly BikeServiceDbContext _bikeServiceDbContext;
    public IBikeRepository BikeRepository { get; }
    public IBikeLocationTrackingRepository BikeLocationTrackingRepository { get; }
    public IAccountRepository AccountRepository { get; }
    public IBikeRentalTrackingHistoryRepository BikeRentalTrackingHistoryRepository { get; }
    public IBikeRentalTrackingRepository BikeRentalTrackingRepository { get; }
    public IBikeStationRepository BikeStationRepository { get; }
    public IBikeStationManagerRepository BikeStationManagerRepository { get; }

    public UnitOfWork(BikeServiceDbContext bikeServiceDbContext)
    {
        _bikeServiceDbContext = bikeServiceDbContext;
        BikeRepository ??= new BikeRepository(bikeServiceDbContext);
        BikeLocationTrackingRepository ??= new BikeLocationTrackingRepository(bikeServiceDbContext);
        AccountRepository ??= new AccountRepository(bikeServiceDbContext);
        BikeRentalTrackingHistoryRepository ??= new BikeRentalTrackingHistoryRepository(bikeServiceDbContext);
        BikeRentalTrackingRepository ??= new BikeRentalTrackingRepository(bikeServiceDbContext);
        BikeStationRepository ??= new BikeStationRepository(bikeServiceDbContext);
        BikeStationManagerRepository ??= new BikeStationManagerRepository(bikeServiceDbContext);
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await _bikeServiceDbContext.SaveChangesAsync();
    }
}
