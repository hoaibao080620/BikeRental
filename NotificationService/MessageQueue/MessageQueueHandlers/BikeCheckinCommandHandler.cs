using System.Text.Json;
using BikeRental.MessageQueue.Commands;
using BikeRental.MessageQueue.Handlers;
using NotificationService.Consts;
using NotificationService.DAL;
using NotificationService.Models;

namespace NotificationService.MessageQueue.MessageQueueHandlers;

public class BikeCheckinCommandHandler : IMessageQueueHandler
{
    private readonly INotificationRepository _notificationRepository;

    public BikeCheckinCommandHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }
    
    public async Task Handle(string message)
    {
        var notificationCommand = JsonSerializer.Deserialize<PushBikeCheckinNotification>(message);
        if(notificationCommand is null) return;

        var tasks = notificationCommand.ManagerEmails.Select(email => 
            _notificationRepository.AddNotification(new Notification
            {
                NotificationEmail = email,
                IsOpen = false,
                IsSeen = false,
                IsHidden = false,
                NotificationContent = string.Format(
                    NotificationMessage.CheckinMessage, 
                    notificationCommand.AccountEmail,
                    notificationCommand.LicensePlate,
                    notificationCommand.BikeStationName,
                    notificationCommand.CheckinOn),
                CreatedOn = DateTime.UtcNow
            })).ToList();

        await Task.WhenAll(tasks);
    }
}
