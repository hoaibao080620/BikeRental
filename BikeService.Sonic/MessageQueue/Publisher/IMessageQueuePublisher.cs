using BikeRental.MessageQueue.Events;

namespace BikeService.Sonic.MessageQueue.Publisher;

public interface IMessageQueuePublisher
{
    Task PublishBikeLocationChangeCommand(List<string> managerEmails);
    Task PublishBikeCheckedInEvent(BikeCheckedIn bikeCheckedIn);
    Task PublishBikeCheckedOutEvent(BikeCheckedOut bikeCheckedOut);
}
