namespace BikeService.Sonic.Services.Interfaces;

public interface ICacheService
{
    Task Add(string key, string value);
    Task Remove(string key);
    Task<string?> Get(string key);
}
