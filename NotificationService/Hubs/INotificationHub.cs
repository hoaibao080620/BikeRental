namespace NotificationService.Hubs;

public interface INotificationHub
{
    Task NotifyBikeLocationHasChanged(string? email);
}
