namespace BikeService.Sonic.Dtos.BikeStation;

public class BikeStationManagerAssignDto
{
    public List<int> BikeStationIds { get; set; } = null!;
    public int ManagerId { get; set; }
}
