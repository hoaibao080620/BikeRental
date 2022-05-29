using BikeService.Sonic.Models;

namespace BikeService.Sonic.Services.Interfaces;

public interface IBikeStationService
{
    List<BikeStation> GetBikeStations();
}