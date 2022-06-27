using NotificationService.Models;

namespace NotificationService.DAL;

public interface INotificationRepository
{
    public Task<List<Notification>> GetNotifications(string email);
    public Task AddNotification(Notification notification);
}
