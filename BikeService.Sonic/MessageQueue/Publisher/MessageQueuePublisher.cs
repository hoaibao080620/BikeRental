using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Publisher;
using Newtonsoft.Json;

namespace BikeService.Sonic.MessageQueue.Publisher;

public class MessageQueuePublisher : IMessageQueuePublisher
{
    private readonly IPublisher _publisher;
    private readonly IConfiguration _configuration;

    public MessageQueuePublisher(IPublisher publisher, IConfiguration configuration)
    {
        _publisher = publisher;
        _configuration = configuration;
    }

    public async Task PublishBikeCreatedEvent(BikeCreated bikeCreated)
    {
        var payload = JsonConvert.SerializeObject(bikeCreated);
        await SendMessage(payload);
    }

    public async Task PublishBikeUpdatedEvent(BikeUpdated bikeUpdated)
    {
        var payload = JsonConvert.SerializeObject(bikeUpdated);
        await SendMessage(payload);
    }

    public async Task PublishBikeDeletedEvent(int bikeId)
    {
        var payload = JsonConvert.SerializeObject(new BikeDeleted
        {
            Id = bikeId
        });
        await SendMessage(payload);
    }

    public async Task PublishBikeStationColorUpdatedEvent(BikeStationColorUpdated bikeStationColorUpdated)
    {
        var payload = JsonConvert.SerializeObject(bikeStationColorUpdated);
        await SendMessage(payload);
    }

    private async Task SendMessage(string payload)
    {
        var topic = _configuration["MessageQueue:BikeTopic"];
        await _publisher.SendMessage(payload, topic);
    }
}
