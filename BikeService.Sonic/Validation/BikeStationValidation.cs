using BikeService.Sonic.DAL;

namespace BikeService.Sonic.Validation;

public class BikeStationValidation : IBikeStationValidation
{
    private readonly IBikeStationRepository _bikeStationRepository;

    public BikeStationValidation(IBikeStationRepository bikeStationRepository)
    {
        _bikeStationRepository = bikeStationRepository;
    }

    public async Task<bool> IsBikeStationHasBikes(int bikeStationId)
    {
        return await _bikeStationRepository.Exists(b => b.Bikes.Any());
    }
}
