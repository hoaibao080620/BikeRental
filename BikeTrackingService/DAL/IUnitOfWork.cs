using BikeService.Sonic.DAL;

namespace BikeTrackingService.DAL;

public interface IUnitOfWork
{
    IBikeRepository BikeRepository { get; }
    IBikeLocationTrackingRepository BikeLocationTrackingRepository { get; }
    IAccountRepository AccountRepository { get; }
    IBikeLocationTrackingHistoryRepository BikeLocationTrackingHistoryRepository { get; }
    IBikeRentalTrackingRepository BikeRentalTrackingRepository { get; }
    Task<int> SaveChangesAsync();
}
