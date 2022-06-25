namespace BikeService.Sonic.Validation;

public interface IBikeStationValidation
{
    Task<bool> IsBikeStationHasBikes(int bikeStationId);
}
