using System.Text.Json;
using BikeRental.MessageQueue.Commands;
using BikeRental.MessageQueue.Handlers;
using NotificationService.Hubs;

namespace NotificationService.MessageQueue.MessageQueueHandlers;

public class NotifyBikeLocationChangeCommandHandler : IMessageQueueHandler
{
    private readonly INotificationHub _notificationHub;

    public NotifyBikeLocationChangeCommandHandler(INotificationHub notificationHub)
    {
        _notificationHub = notificationHub;
    }
    
    public async Task Handle(string message)
    {
        var notifyBikeLocationChange = JsonSerializer.Deserialize<NotifyBikeLocationChange>(message);
        
        if (notifyBikeLocationChange?.ManagerEmails is null) return;
        
        foreach (var email in notifyBikeLocationChange.ManagerEmails)
        {
            await _notificationHub.NotifyBikeLocationHasChanged(email);
        }
    }
}
