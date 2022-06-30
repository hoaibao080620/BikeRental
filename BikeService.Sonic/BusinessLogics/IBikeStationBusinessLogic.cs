using BikeService.Sonic.Dtos;
using BikeService.Sonic.Dtos.BikeStation;

namespace BikeService.Sonic.BusinessLogics;

public interface IBikeStationBusinessLogic
{
    Task<BikeStationRetrieveDto> GetStationBike(int id);
    Task<List<BikeStationRetrieveDto>> GetAllStationBikes();
    Task AddStationBike(BikeStationInsertDto bikeInsertDto);
    Task DeleteStationBike(int id);
    Task UpdateStationBike(BikeStationUpdateDto bikeInsertDto);
    Task<BikeStationRetrieveDto> GetNearestBikeStationFromLocation(double longitude, double latitude);
    Task UpdateBikeStationColor(BikeStationColorsChangeDto bikeStationColors, string email);
    Task<List<BikeStationColorRetrieveDto>> GetBikeStationColors(string email);
}
