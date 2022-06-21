using BikeService.Sonic.Dtos;
using BikeService.Sonic.Dtos.BikeOperation;

namespace BikeService.Sonic.BusinessLogics;

public interface IBikeStationBusinessLogic
{
  Task<BikeStationRetrieveDto> GetStationBike(int id);
  
  Task<List<BikeStationRetrieveDto>> GetAllStationBikes();
  Task AddStationBike(BikeStationInsertDto bikeInsertDto);
  Task DeleteStationBike(int id);

  Task UpdateStationBike(BikeStationUpdateDto bikeInsertDto);
  

}