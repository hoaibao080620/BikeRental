using BikeService.Sonic.Models;

namespace BikeService.Sonic.Services.Interfaces;

public interface IBikeLocationHub
{
    Task SendBikeLocationsData(List<Bike> bikes);
}