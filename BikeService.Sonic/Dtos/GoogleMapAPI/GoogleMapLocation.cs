using System.Text.Json.Serialization;

namespace BikeService.Sonic.Dtos.GoogleMapAPI;

public class GoogleMapLocation
{
    [JsonPropertyName("lat")]
    public double Latitude { get; set; }
    [JsonPropertyName("lng")]
    public double Longitude { get; set; }
}
