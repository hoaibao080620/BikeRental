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
    
    public Task<List<Notification>> GetNotifications(string email)
    {
        var notifications = _notificationCollection.Find(x =>
            x.NotificationEmail == email && x.IsHidden == false).ToList();
        
        return Task.FromResult(notifications);
    }

    public async Task AddNotification(Notification notification)
    {
        await _notificationCollection.InsertOneAsync(notification);
    }
    
    public async Task MarkNotificationSeen(string email)
    {
        var builder = Builders<Notification>.Update;
        await _notificationCollection.UpdateManyAsync(n => n.NotificationEmail == email, builder.Set(
            x => x.IsSeen, true));
    }

    public async Task MarkNotificationOpen(string notificationId)
    {
        var builder = Builders<Notification>.Update;
        await _notificationCollection.UpdateOneAsync(n => n.Id == notificationId, builder.Set(
            x => x.IsOpen, true));
    }
}
