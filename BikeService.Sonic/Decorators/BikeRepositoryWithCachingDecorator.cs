using System.Text.Json;
using BikeService.Sonic.Const;
using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace BikeService.Sonic.Decorators;

public class BikeLoaderAdapterWithCachingDecorator : IBikeLoaderAdapter
{
    private readonly IBikeLoaderAdapter _bikeLoaderAdapter;
    private readonly IDistributedCache _distributedCache;
    private readonly ICacheService _cacheService;

    public BikeLoaderAdapterWithCachingDecorator(
        IBikeLoaderAdapter bikeLoaderAdapter, 
        IDistributedCache distributedCache,
        ICacheService cacheService)
    {
        _bikeLoaderAdapter = bikeLoaderAdapter;
        _distributedCache = distributedCache;
        _cacheService = cacheService;
    }

    public async Task<BikeRetrieveDto> GetBike(int bikeId)
    {
        var key = string.Format(RedisCacheKey.SingleBike, bikeId);
        var cache = await _distributedCache.GetStringAsync(key);
        if (cache is not null)
        {
            var bikes = JsonSerializer.Deserialize<BikeRetrieveDto>(cache);
            return bikes!;
        }

        var bike = await _bikeLoaderAdapter.GetBike(bikeId);
        await _cacheService.Add(key, JsonSerializer.Serialize(bike));

        return bike;
    }

    public async Task<List<int>> GetBikeIdsOfManager(string managerEmail)
    {
        var key = string.Format(RedisCacheKey.ManagerBikeIds, managerEmail);
        var cache = await _distributedCache.GetStringAsync(key);
        if (cache is not null)
        {
            var bikeIds = JsonSerializer.Deserialize<List<int>>(cache);
            return bikeIds!;
        }
        
        var bikeIdsFromDb = await _bikeLoaderAdapter.GetBikeIdsOfManager(managerEmail);
        await _cacheService.Add(key, JsonSerializer.Serialize(bikeIdsFromDb));
        return bikeIdsFromDb;
    }
}
