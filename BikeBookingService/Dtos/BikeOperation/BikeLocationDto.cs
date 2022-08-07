namespace BikeBookingService.Dtos.BikeOperation;

public class BikeLocationDto
{
    public string BikeCode { get; set; } = null!;
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public double Distance { get; set; }
}
