using Newtonsoft.Json;

namespace BikeService.Sonic.Dtos.GoogleMapAPI;

public class GoogleMapLocation
{
    [JsonProperty("lat")]
    public double Latitude { get; set; }
    [JsonProperty("lng")]
    public double Longitude { get; set; }
}
