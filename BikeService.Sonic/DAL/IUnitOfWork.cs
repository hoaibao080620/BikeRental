namespace BikeService.Sonic.DAL;

public interface IUnitOfWork
{
    IBikeRepository BikeRepository { get; }
    IBikeStationRepository BikeStationRepository { get; }
    IBikeStationManagerRepository BikeStationManagerRepository { get; }
    IBikeStationColorRepository BikeStationColorRepository { get; }
    IManagerRepository ManagerRepository { get; }
    IBikeReportRepository BikeReportRepository { get; }
    Task<int> SaveChangesAsync();
}
