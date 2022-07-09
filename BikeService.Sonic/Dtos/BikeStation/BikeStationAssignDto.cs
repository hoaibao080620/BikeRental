namespace BikeService.Sonic.Dtos.BikeStation;

public class BikeStationAssignDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int ParkingSpace { get; set; }
    public int UsedParkingSpace { get; set; }
}
