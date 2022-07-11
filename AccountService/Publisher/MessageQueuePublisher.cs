using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Publisher;
using Newtonsoft.Json;

namespace AccountService.Publisher;

public class MessageQueuePublisher : IMessageQueuePublisher
{
    private readonly IPublisher _publisher;
    private readonly IConfiguration _configuration;

    public MessageQueuePublisher(IPublisher publisher, IConfiguration configuration)
    {
        _publisher = publisher;
        _configuration = configuration;
    }
    

    public async Task PublishBikeCheckedInEvent(BikeCheckedIn bikeCheckedIn)
    {
        var payload = JsonConvert.SerializeObject(bikeCheckedIn);
        var topic = _configuration["MessageQueue:BikeTrackingTopic"];
        await _publisher.SendMessage(payload, topic);
    }

    public async Task PublishBikeCheckedOutEvent(BikeCheckedOut bikeCheckedOut)
    {
        var payload = JsonConvert.SerializeObject(bikeCheckedOut);
        var topic = _configuration["MessageQueue:BikeTrackingTopic"];
        await _publisher.SendMessage(payload, topic);
    }

    public async Task PublishAccountPointSubtractedEvent(AccountPointSubtracted accountPointSubtracted)
    {
        var payload = JsonConvert.SerializeObject(accountPointSubtracted);
        var topic = _configuration["MessageQueue:BikeTrackingTopic"];
        await _publisher.SendMessage(payload, topic);
    }

    public async Task PublishAccountPointLimitExceededEvent(AccountPointLimitExceeded accountPointLimitExceeded)
    {
        var payload = JsonConvert.SerializeObject(accountPointLimitExceeded);
        var topic = _configuration["MessageQueue:BikeTrackingTopic"];
        await _publisher.SendMessage(payload, topic);    }
}
