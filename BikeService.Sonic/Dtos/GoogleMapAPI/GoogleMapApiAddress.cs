using Newtonsoft.Json;

namespace BikeService.Sonic.Dtos.GoogleMapAPI;

public class GoogleMapApiAddress
{
    [JsonProperty("formatted_address")]
    public string FormattedAddress { get; set; } = null!;
}
