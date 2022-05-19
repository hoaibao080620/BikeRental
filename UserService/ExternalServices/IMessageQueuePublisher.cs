using UserService.Models;

namespace UserService.ExternalServices;

public interface IMessageQueuePublisher
{
    Task PublishUserAddedEventToMessageQueue(User user);
    Task PublishUserUpdatedEventToMessageQueue(User user);
    Task PublishUserDeletedEventToMessageQueue(int userId);
}