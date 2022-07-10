using BikeService.Sonic.Const;
using BikeService.Sonic.DAL;

namespace BikeService.Sonic.Validation;

public class BikeStationValidation : IBikeStationValidation
{
    private readonly IUnitOfWork _unitOfWork;

    public BikeStationValidation(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> IsBikeStationHasBikes(int bikeStationId)
    {
        return await _unitOfWork.BikeStationRepository.Exists(b => b.Bikes.Any() && b.Id == bikeStationId);
    }

    public async Task<string?> IsAssignBikesValid(List<int> bikeIds, int bikeStationId)
    {
        var isBikesStatusInvalid = await _unitOfWork.BikeRepository
            .Exists(x => bikeIds.Contains(x.Id) && x.Status == BikeStatus.InUsed);

        if (isBikesStatusInvalid) return "Bike assign have to have available status!";

        var bikeStation = await _unitOfWork.BikeStationRepository.GetById(bikeStationId);
        ArgumentNullException.ThrowIfNull(bikeStation);

        var isBikeStationEnoughSpace = bikeStation.ParkingSpace - bikeStation.UsedParkingSpace > bikeIds.Count;
        return isBikeStationEnoughSpace ? null : "Bike Station doesn't have enough space!";
    }
}
