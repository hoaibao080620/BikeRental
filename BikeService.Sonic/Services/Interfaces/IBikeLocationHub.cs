using BikeService.Sonic.Dtos;

namespace BikeService.Sonic.Services.Interfaces;

public interface IBikeLocationHub
{
    Task SendBikeLocationsData(string? email, BikeLocationDto bike);
}