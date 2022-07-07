namespace BikeService.Sonic.DAL;

public interface IUnitOfWork
{
    IBikeRepository BikeRepository { get; }
    IBikeLocationTrackingRepository BikeLocationTrackingRepository { get; }
    IAccountRepository AccountRepository { get; }
    IBikeRentalTrackingHistoryRepository BikeRentalTrackingHistoryRepository { get; }
    IBikeRentalTrackingRepository BikeRentalTrackingRepository { get; }
    IBikeStationRepository BikeStationRepository { get; }
    IBikeStationManagerRepository BikeStationManagerRepository { get; }
    IBikeStationColorRepository BikeStationColorRepository { get; }
    IManagerRepository ManagerRepository { get; }
    IBikeReportRepository BikeReportRepository { get; }
    Task<int> SaveChangesAsync();
}
