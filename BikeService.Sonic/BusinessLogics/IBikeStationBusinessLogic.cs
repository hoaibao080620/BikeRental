using BikeService.Sonic.Dtos;
using BikeService.Sonic.Dtos.BikeOperation;
using BikeService.Sonic.Models;

namespace BikeService.Sonic.BusinessLogics;

public interface IBikeStationBusinessLogic
{
    Task<BikeStationRetrieveDto> GetStationBike(int id);
    Task<List<BikeStationRetrieveDto>> GetAllStationBikes();
    Task AddStationBike(BikeStationInsertDto bikeInsertDto);
    Task DeleteStationBike(int id);
    Task UpdateStationBike(BikeStationUpdateDto bikeInsertDto);
    Task<BikeStationRetrieveDto> GetNearestBikeStationFromLocation(double longitude, double latitude);
}
