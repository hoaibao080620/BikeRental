using BikeService.Sonic.DAL;
using BikeTrackingService.Dtos.History;

namespace BikeTrackingService.BLL;

public class BikeTrackingBusinessLogic : IBikeTrackingBusinessLogic
{
    private readonly BikeServiceGrpc.BikeServiceGrpcClient _bikeServiceGrpc;
    private readonly IUnitOfWork _unitOfWork;

    public BikeTrackingBusinessLogic(BikeServiceGrpc.BikeServiceGrpcClient bikeServiceGrpc, IUnitOfWork unitOfWork)
    {
        _bikeServiceGrpc = bikeServiceGrpc;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<List<BikeRentingHistory>> GetBikeRentingHistories(string email)
    {
        var getBikeIdsResponse = await _bikeServiceGrpc.GetBikeIdsOfManagerAsync(new GetBikeIdsRequest
        {
            ManagerEmail = email
        });
        ArgumentNullException.ThrowIfNull(getBikeIdsResponse);

        var bikeRentingHistories = (await _unitOfWork.BikeRentalTrackingRepository
            .Find(x =>
                getBikeIdsResponse.BikeIds.Contains(x.Bike.ExternalId)
            )).Select(x => new BikeRentingHistory
            {
                Id = x.Id,
                BikeId = x.BikeId,
                BikePlate = x.Bike.LicensePlate,
                AccountEmail = x.Account.Email,
                CheckedInOn = x.CheckinOn,
                CheckedOutOn = x.CheckoutOn,
                TotalTime = x.CheckoutOn.HasValue ? x.CheckoutOn.Value.Subtract(x.CheckinOn).TotalMinutes : null
            }).ToList();

        return bikeRentingHistories;
    }
}
