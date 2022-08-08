using BikeBookingService.BikeTrackingServiceDbContext;
using BikeService.Sonic.DAL;

namespace BikeBookingService.DAL;

public class UnitOfWork : IUnitOfWork
{
    private readonly BikeTrackingDbContext _bikeTrackingDbContext;
    public IBikeRepository BikeRepository { get; }
    public IBikeLocationTrackingRepository BikeLocationTrackingRepository { get; }
    public IAccountRepository AccountRepository { get; }
    public IBikeLocationTrackingHistoryRepository BikeLocationTrackingHistoryRepository { get; }
    public IBikeRentalTrackingRepository BikeRentalTrackingRepository { get; }
    public IRentingPointRepository RentingPointRepository { get; }
    public IRentingPointHistoryRepository RentingPointHistoryRepository { get; }


    public UnitOfWork(BikeTrackingDbContext bikeTrackingDbContext)
    {
        _bikeTrackingDbContext = bikeTrackingDbContext;
        BikeRepository ??= new BikeRepository(bikeTrackingDbContext);
        BikeLocationTrackingRepository ??= new BikeLocationTrackingRepository(bikeTrackingDbContext);
        AccountRepository ??= new AccountRepository(bikeTrackingDbContext);
        BikeLocationTrackingHistoryRepository ??= new BikeLocationTrackingHistoryRepository(bikeTrackingDbContext);
        BikeRentalTrackingRepository ??= new BikeRentalTrackingRepository(bikeTrackingDbContext);
        RentingPointRepository ??= new RentingPointRepository(bikeTrackingDbContext);
        RentingPointHistoryRepository ??= new RentingPointHistoryRepository(bikeTrackingDbContext);
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await _bikeTrackingDbContext.SaveChangesAsync();
    }
}
