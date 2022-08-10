using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using Newtonsoft.Json;
using NotificationService.Hubs;

namespace NotificationService.MessageQueue.MessageQueueHandlers;

public class UserRunOutPointHandler : IMessageQueueHandler
{
    private readonly INotificationHub _notificationHub;

    public UserRunOutPointHandler(INotificationHub notificationHub)
    {
        _notificationHub = notificationHub;
    }
    
    public async Task Handle(string message)
    {
        var payload = JsonConvert.DeserializeObject<UserAlmostRunOutPoint>(message);
        
        if (payload is null) return;

        await _notificationHub.PushPointRunOutNotification(payload.Email, payload.Message);
    }
}
