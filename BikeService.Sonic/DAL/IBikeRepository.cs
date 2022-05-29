using BikeService.Sonic.Models;

namespace BikeService.Sonic.DAL;

public interface IBikeRepository
{
    Task<List<Bike>> GetAll();
    Task<Bike> GetById(string id);
    Task Add(Bike bike);
    Task Update(string id, Bike bike);
    Task Delete(string id);
}