namespace BikeBookingService.Dtos;

public class RentingPointUpdateDto
{
    public double CurrentPoint { get; set; }
    public double UpdatePoint { get; set; }
    public string? ChangeReason { get; set; }
}
