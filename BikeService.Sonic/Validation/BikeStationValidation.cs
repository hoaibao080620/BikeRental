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

        if (isBikesStatusInvalid) return "Xe bạn chọn phải đang có trạng thái sẵn sàng và chưa được thuê!";

        var bikeStation = await _unitOfWork.BikeStationRepository.GetById(bikeStationId);
        var bikesInBikeStation = (await _unitOfWork.BikeRepository.Find(x => x.BikeStationId == bikeStationId)).Count();
        ArgumentNullException.ThrowIfNull(bikeStation);

        var isBikeStationEnoughSpace = bikeStation.ParkingSpace - bikesInBikeStation > bikeIds.Count;
        return isBikeStationEnoughSpace ? null : "Trạm không đủ chỗ để xe!";
    }
}
