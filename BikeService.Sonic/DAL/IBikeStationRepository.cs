using BikeService.Sonic.Models;

namespace BikeService.Sonic.DAL;

public interface IBikeStationRepository
{
    Task<List<BikeStation>> GetAll();
    Task<BikeStation> GetById(string id);
    Task Add(BikeStation bikeStation);
    Task Update(string id, BikeStation bikeStation);
    Task Delete(string id);
}