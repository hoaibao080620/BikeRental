using BikeService.Sonic.DAL;
using Grpc.Core;

namespace BikeService.Sonic.GrpcServices;

public class BikeGrpcService : BikeServiceGrpc.BikeServiceGrpcBase
{
    private readonly IUnitOfWork _unitOfWork;

    public BikeGrpcService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public override async Task<GetBikeIdsResponse> GetBikeIdsOfManager(GetBikeIdsRequest request, ServerCallContext context)
    {
        var manager = (await _unitOfWork.ManagerRepository
            .Find(x => x.Email == request.ManagerEmail)).FirstOrDefault();

        ArgumentNullException.ThrowIfNull(manager);
        
        var bikeIds = (await _unitOfWork.BikeRepository
            .Find(x =>
                x.BikeStationId.HasValue &&
                manager.IsSuperManager || 
                x.BikeStation!.BikeStationManagers.Any(b => b.Manager.Email == request.ManagerEmail)
            ))
            .Select(x => x.Id).ToList();

        return new GetBikeIdsResponse
        {
            BikeIds =
            {
                bikeIds
            }
        };
    }
}
