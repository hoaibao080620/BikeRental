using BikeService.Sonic.DAL;

namespace BikeBookingService.DAL;

public interface IUnitOfWork
{
    IBikeRepository BikeRepository { get; }
    IBikeLocationTrackingRepository BikeLocationTrackingRepository { get; }
    IAccountRepository AccountRepository { get; }
    IBikeLocationTrackingHistoryRepository BikeLocationTrackingHistoryRepository { get; }
    IBikeRentalTrackingRepository BikeRentalTrackingRepository { get; }
    IRentingPointRepository RentingPointRepository { get; }
    IRentingPointHistoryRepository RentingPointHistoryRepository { get; }
    Task<int> SaveChangesAsync();
}
