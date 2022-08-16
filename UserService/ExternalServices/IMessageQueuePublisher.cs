using BikeRental.MessageQueue.Events;
using UserService.Models;

namespace UserService.ExternalServices;

public interface IMessageQueuePublisher
{
    Task PublishUserAddedEventToMessageQueue(User user);
    Task PublishUserUpdatedEventToMessageQueue(User user, string? imageBase64);
    Task PublishUserDeletedEventToMessageQueue(User user);
    Task PublishUserRoleUpdatedEvent(UserRoleUpdated userRoleUpdated);
    Task PublishUserDeactivatedEvent(UserDeactivated userDeactivated);
    Task PublishUserActivatedEvent(UserReactivated userReactivated);
}
