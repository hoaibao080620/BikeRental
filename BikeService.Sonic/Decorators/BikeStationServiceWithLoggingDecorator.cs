using BikeService.Sonic.Models;
using BikeService.Sonic.Services.Interfaces;

namespace BikeService.Sonic.Decorators;

public class BikeStationServiceWithLoggingDecorator : IBikeStationService
{
    private readonly IBikeStationService _bikeStationService;
    private readonly ILogger<BikeStationServiceWithLoggingDecorator> _logger;

    public BikeStationServiceWithLoggingDecorator(
        IBikeStationService bikeStationService, 
        ILogger<BikeStationServiceWithLoggingDecorator> logger)
    {
        _bikeStationService = bikeStationService;
        _logger = logger;
    }
    
    public List<BikeStation> GetBikeStations()
    {
        var bikeStations = _bikeStationService.GetBikeStations();
        _logger.LogInformation("Get bike stations success!");

        return bikeStations;
    }
}