using BikeService.Sonic.Dtos.GoogleMapAPI;
using BikeService.Sonic.Services.Interfaces;
using Newtonsoft.Json;

namespace BikeService.Sonic.Services.Implementation;

public class GoogleMapService : IGoogleMapService
{
    private readonly HttpClient _httpClient;
    private readonly string _geolocationBaseUrl;
    private readonly string _apiKey;
    private readonly string _distanceBaseUrl;

    public GoogleMapService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _geolocationBaseUrl = configuration["GoogleMapAPI:Geolocation"];
        _distanceBaseUrl = configuration["GoogleMapAPI:Distance"];
        _apiKey = configuration["GoogleMapAPI:API_KEY"];
    }
    
    public async Task<string?> GetAddressOfLocation(double longitude, double latitude)
    {
        var endpoint = $"{_geolocationBaseUrl}?latlng={latitude},{longitude}&key={_apiKey}";
        var response = await (await _httpClient.PostAsync(endpoint, null)).Content.ReadAsStringAsync();
        var address = JsonConvert.DeserializeObject<GoogleMapApiResponse>(response);

        return address?.Results.FirstOrDefault()?.FormattedAddress;
    }

    public async Task<(double, double)> GetLocationOfAddress(string placeId)
    {
        var endpoint = $"{_geolocationBaseUrl}?place_id={placeId}&key={_apiKey}";
        var response = await (await _httpClient.PostAsync(endpoint, null)).Content.ReadAsStringAsync();
        var location = JsonConvert.DeserializeObject<GoogleMapPlaceApiAddress>(response)?.Results.FirstOrDefault()!;

        return (location.Geometry.Location.Latitude, location.Geometry.Location.Longitude);
    }

    public async Task<double> GetDistanceBetweenTwoLocations(GoogleMapLocation origin, GoogleMapLocation destination)
    {
        var endpoint = $"{_distanceBaseUrl}?origins={origin.Latitude},{origin.Longitude}" +
                       $"&destinations={destination.Latitude},{destination.Longitude}&key={_apiKey}";
        var response = await _httpClient.GetStringAsync(endpoint);

        var distance = JsonConvert.DeserializeObject<GoogleDistanceApiResponse>(response)?
            .Rows.FirstOrDefault()?.Elements.FirstOrDefault()?.Distance.Value;

        return distance ?? default;
    }
}
