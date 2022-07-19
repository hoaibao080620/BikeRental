using System.Text.Json;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using NotificationService.Consts;
using NotificationService.DAL;
using NotificationService.Hubs;
using NotificationService.Models;

namespace NotificationService.MessageQueue.MessageQueueHandlers;

public class BikeCheckinCommandHandler : IMessageQueueHandler
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationHub _notificationHub;

    public BikeCheckinCommandHandler(INotificationRepository notificationRepository, INotificationHub notificationHub)
    {
        _notificationRepository = notificationRepository;
        _notificationHub = notificationHub;
    }
    
    public async Task Handle(string message)
    {
        var notificationCommand = JsonSerializer.Deserialize<BikeCheckedIn>(message);
        if(notificationCommand is null) return;

        var tasks = new List<Task>();
        foreach (var email in notificationCommand.ManagerEmails)
        {
            var notification = new Notification
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
                CreatedOn = DateTime.Now,
                NotificationType = NotificationType.Checkin
            };
            
            tasks.Add(_notificationRepository.AddNotification(notification));
            tasks.Add(_notificationHub.PushNotification(email, notification));
        }

        await Task.WhenAll(tasks);
    }
}
