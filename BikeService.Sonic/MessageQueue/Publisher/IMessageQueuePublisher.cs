using BikeRental.MessageQueue.Events;

namespace BikeService.Sonic.MessageQueue.Publisher;

public interface IMessageQueuePublisher
{
    Task PublishBikeCreatedEvent(BikeCreated bikeCreated);
    Task PublishBikeUpdatedEvent(BikeUpdated bikeUpdated);
    Task PublishBikeDeletedEvent(int bikeId);
    Task PublishBikeStationColorUpdatedEvent(BikeStationColorUpdated bikeStationColorUpdated);
}
