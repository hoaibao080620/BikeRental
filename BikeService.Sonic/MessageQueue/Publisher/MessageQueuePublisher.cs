using BikeRental.MessageQueue.Commands;
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

        var topic = _configuration["MessageQueue:NotificationTopic"];
        await _publisher.SendMessage(payload, topic);
    }

    public async Task PublishBikeCheckinNotificationCommand(PushBikeCheckinNotification bikeCheckinNotification)
    {
        var payload = JsonConvert.SerializeObject(bikeCheckinNotification);
        var topic = _configuration["MessageQueue:NotificationTopic"];
        await _publisher.SendMessage(payload, topic);
    }

    public async Task PublishBikeCheckoutNotificationCommand(PushBikeCheckoutNotification bikeCheckoutNotification)
    {
        var payload = JsonConvert.SerializeObject(bikeCheckoutNotification);
        var topic = _configuration["MessageQueue:NotificationTopic"];
        await _publisher.SendMessage(payload, topic);
    }
}
