using System.Text.Json;
using BikeRental.MessageQueue.Commands;
using BikeRental.MessageQueue.Handlers;
using NotificationService.Hub;

namespace NotificationService.MessageQueue.MessageQueueHandlers;

public class NotifyBikeLocationChangeCommandHandler : IMessageQueueHandler
{
    private readonly IBikeLocationHub _bikeLocationHub;

    public NotifyBikeLocationChangeCommandHandler(IBikeLocationHub bikeLocationHub)
    {
        _bikeLocationHub = bikeLocationHub;
    }
    
    public async Task Handle(string message)
    {
        var notifyBikeLocationChange = JsonSerializer.Deserialize<NotifyBikeLocationChange>(message);

        if (notifyBikeLocationChange?.ManagerEmails is null) return;

        foreach (var email in notifyBikeLocationChange.ManagerEmails)
        {
            await _bikeLocationHub.NotifyBikeLocationHasChanged(email);
        }
    }
}
