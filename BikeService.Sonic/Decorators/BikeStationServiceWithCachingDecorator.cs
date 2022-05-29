using BikeService.Sonic.Models;
using BikeService.Sonic.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace BikeService.Sonic.Decorators;

public class BikeStationServiceWithCachingDecorator : IBikeStationService
{
    private readonly IBikeStationService _bikeStationService;
    private readonly IDistributedCache _distributedCache;

    public BikeStationServiceWithCachingDecorator(IBikeStationService bikeStationService, IDistributedCache distributedCache)
    {
        _bikeStationService = bikeStationService;
        _distributedCache = distributedCache;
    }
    
    public List<BikeStation> GetBikeStations()
    {
        throw new NotImplementedException();
    }
}