using BikeRental.MessageQueue.Commands;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.MessageType;
using BikeRental.MessageQueue.Publisher;
using Newtonsoft.Json;

namespace BikeTrackingService.MessageQueue.Publisher;

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
}
