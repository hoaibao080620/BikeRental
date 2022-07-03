using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Publisher;
using Newtonsoft.Json;

namespace PaymentService.MessageQueue;

public class MessageQueuePublisher : IMessageQueuePublisher
{
    private readonly IPublisher _publisher;
    private readonly IConfiguration _configuration;

    public MessageQueuePublisher(IPublisher publisher, IConfiguration configuration)
    {
        _publisher = publisher;
        _configuration = configuration;
    }
    
    public async Task PublishPaymentSucceededEvent(PaymentSucceeded paymentSucceeded)
    {
        var topic = _configuration["MessageQueue:PaymentTopic"];
        var payload = JsonConvert.SerializeObject(paymentSucceeded);
        await _publisher.SendMessage(payload, topic);
    }

    public async Task PublishPaymentFailedEvent(PaymentFailed paymentFailed)
    {
        var topic = _configuration["MessageQueue:PaymentTopic"];
        var payload = JsonConvert.SerializeObject(paymentFailed);
        await _publisher.SendMessage(payload, topic);
    }
}
