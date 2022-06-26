namespace NotificationService.Hub;

public interface IBikeLocationHub
{
    Task NotifyBikeLocationHasChanged(string? email);
}
