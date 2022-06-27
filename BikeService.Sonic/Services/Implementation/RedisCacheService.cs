using System.Text;
using BikeService.Sonic.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace BikeService.Sonic.Services.Implementation;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;

    public RedisCacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }
    
    public async Task Add(string key, string value)
    {
        await _distributedCache.SetAsync(
            key, 
            Encoding.ASCII.GetBytes(value), 
            new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(1),
                SlidingExpiration = TimeSpan.FromMinutes(45)
            });
    }

    public async Task Remove(string key)
    {
        await _distributedCache.RemoveAsync(key);
    }

    public async Task<string?> Get(string key)
    {
        return await _distributedCache.GetStringAsync(key);
    }
}
