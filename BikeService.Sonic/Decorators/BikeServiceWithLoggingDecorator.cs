// using BikeService.Sonic.Models;
// using BikeService.Sonic.Services.Interfaces;
//
// namespace BikeService.Sonic.Decorators;
//
// public class BikeServiceWithLoggingDecorator : IBikeService
// {
//     private readonly IBikeService _bikeService;
//     private readonly ILogger<BikeServiceWithLoggingDecorator> _logger;
//
//     public BikeServiceWithLoggingDecorator(
//         IBikeService bikeService, 
//         ILogger<BikeServiceWithLoggingDecorator> logger)
//     {
//         _bikeService = bikeService;
//         _logger = logger;
//     }
//     
//     public List<BikeStation> GetBikeStations()
//     {
//         var bikeStations = _bikeService.GetBikes();
//         _logger.LogInformation("Get bike stations success!");
//
//         return bikeStations;
//     }
// }
