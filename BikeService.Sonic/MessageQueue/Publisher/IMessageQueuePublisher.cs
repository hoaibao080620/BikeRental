using BikeRental.MessageQueue.Commands;

namespace BikeService.Sonic.MessageQueue.Publisher;

public interface IMessageQueuePublisher
{
    Task PublishBikeLocationChangeCommand(List<string> managerEmails);
    Task PublishBikeCheckinNotificationCommand(PushBikeCheckinNotification bikeCheckinNotification);
    Task PublishBikeCheckoutNotificationCommand(PushBikeCheckoutNotification bikeCheckoutNotification);
}
