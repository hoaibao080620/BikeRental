using System.Linq.Expressions;
using NotificationService.Models;

namespace NotificationService.DAL;

public interface INotificationRepository
{
    public Task<List<Notification>> GetNotifications(string email);
    public Task AddNotification(Notification notification);
    public Task MarkNotificationSeen(string email);
    public Task MarkNotificationOpen(string notificationId);
    public Task AddCall(Call call);
    public Task AddRecordingUrlToCall(string callSid, string recordingUrl);
    public Task<List<Call>> GetCalls(Expression<Func<Call, bool>> filter);
}
