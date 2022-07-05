namespace BikeService.Sonic.Dtos.BikeStation;

public class BikeStationRetrieveParameter
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Limit { get; set; } = 1000;
}
