using BikeService.Sonic.Dtos.GoogleMapAPI;
using BikeService.Sonic.Services.Interfaces;
using Newtonsoft.Json;

namespace BikeService.Sonic.Services.Implementation;

public class GoogleMapService : IGoogleMapService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _apiKey;

    public GoogleMapService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseUrl = configuration["GoogleMapAPI:URL"];
        _apiKey = configuration["GoogleMapAPI:API_KEY"];
    }
    
    public async Task<string?> GetAddressOfLocation(double longitude, double latitude)
    {
        var endpoint = $"{_baseUrl}?latlng={latitude},{longitude}&key={_apiKey}";
        var response = await (await _httpClient.PostAsync(endpoint, null)).Content.ReadAsStringAsync();
        var address = JsonConvert.DeserializeObject<GoogleMapApiResponse>(response);

        return address?.Results.FirstOrDefault()?.FormattedAddress;
    }

    public async Task<(double, double)> GetLocationOfAddress(string placeId)
    {
        var endpoint = $"{_baseUrl}?place_id={placeId}&key={_apiKey}";
        var response = await (await _httpClient.PostAsync(endpoint, null)).Content.ReadAsStringAsync();
        var location = JsonConvert.DeserializeObject<GoogleMapPlaceApiAddress>(response)?.Results.FirstOrDefault()!;

        return (location.Geometry.Location.Latitude, location.Geometry.Location.Longitude);
    }
}
