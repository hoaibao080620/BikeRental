namespace NotificationService.Services;

public interface IMessageService
{
    public Task SendMessage(string to, string body);
}
