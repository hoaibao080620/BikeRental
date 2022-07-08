using BikeService.Sonic.DAL;
using BikeTrackingService.BikeTrackingServiceDbContext;

namespace BikeTrackingService.DAL;

public class UnitOfWork : IUnitOfWork
{
    private readonly BikeTrackingDbContext _bikeTrackingDbContext;
    public IBikeRepository BikeRepository { get; }
    public IBikeLocationTrackingRepository BikeLocationTrackingRepository { get; }
    public IAccountRepository AccountRepository { get; }
    public IBikeRentalTrackingHistoryRepository BikeRentalTrackingHistoryRepository { get; }
    public IBikeRentalTrackingRepository BikeRentalTrackingRepository { get; }


    public UnitOfWork(BikeTrackingDbContext bikeTrackingDbContext)
    {
        _bikeTrackingDbContext = bikeTrackingDbContext;
        BikeRepository ??= new BikeRepository(bikeTrackingDbContext);
        BikeLocationTrackingRepository ??= new BikeLocationTrackingRepository(bikeTrackingDbContext);
        AccountRepository ??= new AccountRepository(bikeTrackingDbContext);
        BikeRentalTrackingHistoryRepository ??= new BikeRentalTrackingHistoryRepository(bikeTrackingDbContext);
        BikeRentalTrackingRepository ??= new BikeRentalTrackingRepository(bikeTrackingDbContext);
    }
    
    public async Task<int> SaveChangesAsync()
    {
        return await _bikeTrackingDbContext.SaveChangesAsync();
    }
}
