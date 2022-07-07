namespace BikeService.Sonic.Validation;

public interface IBikeStationValidation
{
    Task<bool> IsBikeStationHasBikes(int bikeStationId);
    Task<string?> IsAssignBikesValid(List<int> bikeIds, int bikeStationId);
}
