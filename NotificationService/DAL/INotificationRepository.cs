using NotificationService.Models;

namespace NotificationService.DAL;

public interface INotificationRepository
{
    public Task<List<Notification>> GetNotifications(string email);
    public Task AddNotification(Notification notification);
    public Task MarkNotificationSeen(string email);
    public Task MarkNotificationOpen(string notificationId);
}
