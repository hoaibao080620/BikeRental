using System.Linq.Expressions;
using MongoDB.Driver;
using NotificationService.Consts;
using NotificationService.Models;

namespace NotificationService.DAL;

public class NotificationRepository : INotificationRepository
{
    private readonly IMongoCollection<Notification> _notificationCollection;
    private readonly IMongoCollection<Call> _callCollection;

    public NotificationRepository(IMongoDatabase mongoDatabase)
    {
        _notificationCollection = mongoDatabase.GetCollection<Notification>(MongoDbCollection.Notification);
        _callCollection = mongoDatabase.GetCollection<Call>(MongoDbCollection.Call);
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

    public async Task AddCall(Call call)
    {
        await _callCollection.InsertOneAsync(call);
    }

    public async Task AddRecordingUrlToCall(string callSid, string recordingUrl)
    {
        var builder = Builders<Call>.Update;
        await _callCollection.UpdateOneAsync(n => n.CallSid == callSid, builder.Set(
            x => x.RecordingUrl, recordingUrl));
    }

    public async Task<List<Call>> GetCalls(Expression<Func<Call, bool>> filter)
    {
        var calls = await _callCollection.FindAsync(filter);
        return calls.ToList().OrderByDescending(x => x.CalledOn).ToList();
    }
}
