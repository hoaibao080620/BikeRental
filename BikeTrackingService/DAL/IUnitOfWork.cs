using BikeTrackingService.DAL;

namespace BikeService.Sonic.DAL;

public interface IUnitOfWork
{
    IBikeRepository BikeRepository { get; }
    IBikeLocationTrackingRepository BikeLocationTrackingRepository { get; }
    IAccountRepository AccountRepository { get; }
    IBikeRentalTrackingHistoryRepository BikeRentalTrackingHistoryRepository { get; }
    IBikeRentalTrackingRepository BikeRentalTrackingRepository { get; }
    Task<int> SaveChangesAsync();
}
