using BikeRental.MessageQueue.Commands;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.MessageType;
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
    
    public async Task PublishBikeLocationChangeCommand(List<string> managerEmails)
    {
        var payload = JsonConvert.SerializeObject(new NotifyBikeLocationChange
        {
            ManagerEmails = managerEmails,
            MessageType = MessageType.NotifyBikeLocationChange
        });

        var topic = _configuration["MessageQueue:BikeTopic"];
        await _publisher.SendMessage(payload, topic);
    }

    public async Task PublishBikeCheckedInEvent(BikeCheckedIn bikeCheckedIn)
    {
        var payload = JsonConvert.SerializeObject(bikeCheckedIn);
        var topic = _configuration["MessageQueue:BikeTopic"];
        await _publisher.SendMessage(payload, topic);
    }

    public async Task PublishBikeCheckedOutEvent(BikeCheckedOut bikeCheckedOut)
    {
        var payload = JsonConvert.SerializeObject(bikeCheckedOut);
        var topic = _configuration["MessageQueue:BikeTopic"];
        await _publisher.SendMessage(payload, topic);
    }

    public async Task PublishBikeCreatedEvent(BikeCreated bikeCreated)
    {
        var payload = JsonConvert.SerializeObject(bikeCreated);
        var topic = _configuration["MessageQueue:BikeTopic"];
        await _publisher.SendMessage(payload, topic);
    }

    public async Task PublishBikeUpdatedEvent(BikeUpdated bikeUpdated)
    {
        var payload = JsonConvert.SerializeObject(bikeUpdated);
        var topic = _configuration["MessageQueue:BikeTopic"];
        await _publisher.SendMessage(payload, topic);
    }

    public async Task PublishBikeDeletedEvent(int bikeId)
    {
        var payload = JsonConvert.SerializeObject(new BikeDeleted
        {
            Id = bikeId
        });
        var topic = _configuration["MessageQueue:BikeTopic"];
        await _publisher.SendMessage(payload, topic);
    }
}
