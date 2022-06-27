using MongoDB.Driver;
using NotificationService.Consts;
using NotificationService.Models;

namespace NotificationService.DAL;

public class NotificationRepository : INotificationRepository
{
    private readonly IMongoCollection<Notification> _notificationCollection;

    public NotificationRepository(IMongoDatabase mongoDatabase)
    {
        _notificationCollection = mongoDatabase.GetCollection<Notification>(MongoDbCollection.Notification);
    }
    
    public async Task<List<Notification>> GetNotifications(string email)
    {
        var notifications = await _notificationCollection.Find(x =>
            x.NotificationEmail == email && x.IsHidden == false).ToListAsync();
        return notifications;
    }

    public async Task AddNotification(Notification notification)
    {
        await _notificationCollection.InsertOneAsync(notification);
    }
}
