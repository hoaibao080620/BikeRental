using System.Text;
using System.Text.Json;
using BikeService.Sonic.Const;
using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace BikeService.Sonic.Decorators;

public class BikeRepositoryAdapterWithCachingDecorator : IBikeRepositoryAdapter
{
    private readonly IBikeRepositoryAdapter _bikeRepositoryAdapter;
    private readonly IDistributedCache _distributedCache;
    private readonly IGoogleMapService _googleMapService;

    public BikeRepositoryAdapterWithCachingDecorator(
        IBikeRepositoryAdapter bikeRepositoryAdapter, 
        IDistributedCache distributedCache,
        IGoogleMapService googleMapService)
    {
        _bikeRepositoryAdapter = bikeRepositoryAdapter;
        _distributedCache = distributedCache;
        _googleMapService = googleMapService;
    }

    public async Task<BikeRetrieveDto?> GetBike(int bikeId)
    {
        var key = string.Format(RedisCacheKey.SingleBikeStation, bikeId);
        var cache = await _distributedCache.GetStringAsync(key);
        if (cache is not null)
        {
            var bikes = JsonSerializer.Deserialize<BikeRetrieveDto>(cache);
            return bikes!;
        }

        var bike = await _bikeRepositoryAdapter.GetBike(bikeId);

        if (bike?.LastLongitude != null || bike?.LastLatitude != null || !string.IsNullOrEmpty(bike?.LastAddress))
        {
            bike.LastAddress =
                await _googleMapService.GetAddressOfLocation(bike.LastLongitude!.Value,
                    bike.LastLatitude!.Value);
        }
        
        await _distributedCache.SetAsync(
            key, 
            Encoding.ASCII.GetBytes(JsonSerializer.Serialize(bike)), 
            new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(1)
            });
        return bike;
    }

    public async Task<List<BikeRetrieveDto>> GetBikes(string managerEmail)
    {
        var cache = await _distributedCache.GetStringAsync(RedisCacheKey.BikeStationIds);
        if (cache is not null)
        {
            var bikes = JsonSerializer.Deserialize<List<BikeRetrieveDto>>(cache);
            return bikes!;
        }
        
        var bikesFromDb = await _bikeRepositoryAdapter.GetBikes(managerEmail);
        foreach (var bike in bikesFromDb)
        {
            if(!bike.LastLongitude.HasValue || !bike.LastLatitude.HasValue || !string.IsNullOrEmpty(bike.LastAddress)) continue;
            
            bike.LastAddress =
                await _googleMapService.GetAddressOfLocation(bike.LastLongitude!.Value,
                    bike.LastLatitude!.Value);
        }
        
        await _distributedCache.SetAsync(
            RedisCacheKey.BikeStationIds, 
            Encoding.ASCII.GetBytes( JsonSerializer.Serialize(bikesFromDb)), 
            new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(1),
                SlidingExpiration = TimeSpan.FromMinutes(45)
            });

        return bikesFromDb;
    }
}
