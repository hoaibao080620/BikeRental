namespace NotificationService.Hubs;

public interface IBikeLocationHub
{
    Task NotifyBikeLocationHasChanged(string? email);
}
