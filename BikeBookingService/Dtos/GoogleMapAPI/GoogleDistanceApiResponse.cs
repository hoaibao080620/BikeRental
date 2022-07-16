#nullable disable
using Newtonsoft.Json;

namespace BikeBookingService.Dtos.GoogleMapAPI;

public class Distance
{
    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("value")]
    public int Value { get; set; }
}

public class Duration
{
    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("value")]
    public int Value { get; set; }
}

public class Element
{
    [JsonProperty("distance")]
    public Distance Distance { get; set; }

    [JsonProperty("duration")]
    public Duration Duration { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }
}

public class GoogleDistanceApiResponse
{
    [JsonProperty("destination_addresses")]
    public List<string> DestinationAddresses { get; set; }

    [JsonProperty("origin_addresses")]
    public List<string> OriginAddresses { get; set; }

    [JsonProperty("rows")]
    public List<Row> Rows { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }
}

public class Row
{
    [JsonProperty("elements")]
    public List<Element> Elements { get; set; }
}
