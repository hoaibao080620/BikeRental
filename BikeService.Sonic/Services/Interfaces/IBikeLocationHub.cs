using BikeService.Sonic.Dtos;
using BikeService.Sonic.Dtos.BikeOperation;

namespace BikeService.Sonic.Services.Interfaces;

public interface IBikeLocationHub
{
    Task SendBikeLocationsData(string? email, BikeLocationDto bike);
}