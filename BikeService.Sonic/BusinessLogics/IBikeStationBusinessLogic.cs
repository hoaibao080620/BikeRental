using BikeService.Sonic.Dtos;
using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Dtos.BikeStation;

namespace BikeService.Sonic.BusinessLogics;

public interface IBikeStationBusinessLogic
{
    Task<BikeStationRetrieveDto> GetStationBike(int id);
    Task<List<BikeStationRetrieveDto>> GetAllStationBikes(string managerEmail);
    Task AddStationBike(BikeStationInsertDto bikeInsertDto);
    Task DeleteStationBike(int id);
    Task UpdateStationBike(BikeStationUpdateDto bikeInsertDto);
    Task UpdateBikeStationColor(List<BikeStationColorDto> bikeStationColors, string email);
    Task<List<BikeStationColorRetrieveDto>> GetBikeStationColors(string email);
    Task<List<BikeRetrieveDto>> GetBikeStationBike(int bikeStationId);
    Task<List<BikeStationNearMeDto>> GetBikeStationsNearMe(BikeStationRetrieveParameter bikeStationRetrieveParameter, string email);
    Task AssignBikesToBikeStation(BikeStationBikeAssignDto bikeAssignDto);
    Task<List<BikeStationAssignDto>> GetAssignableBikeStations(int totalBikeAssign);
    Task<List<AssignableManager>> GetAssignableManagers();
    Task<List<AssignableStation>> GetAssignableStations();
    Task AssignBikeStationsToManager(BikeStationManagerAssignDto bikeStationManagerAssign);
}
