using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using NotificationService.DAL;

namespace NotificationService.GrpcService;

public class NotificationGrpcService : NotificationServiceGrpc.NotificationServiceGrpcBase
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationGrpcService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }
    
    public override async Task<GetCallStatisticResponse> GetCallStatistic(Empty request, ServerCallContext context)
    {
        var calls = await _notificationRepository.GetCalls(_ => true);
        var callIncoming = calls.Count(x => x.Direction == "inbound");
        var callCount = calls.Count;

        var callIncomingRate = callIncoming / callCount;
        return new GetCallStatisticResponse
        {
            IncomingCallRate = callIncomingRate,
            OutgoingCallRate = 100 - callIncomingRate
        };
    }
}
