using NotificationService.Models;

namespace NotificationService.Hubs;

public interface INotificationHub
{
    Task NotifyBikeLocationHasChanged(string? email);
    Task PushNotification(string? email, Notification notification);
}
