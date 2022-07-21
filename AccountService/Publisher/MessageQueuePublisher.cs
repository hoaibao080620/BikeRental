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

    public async Task PublishAccountPointSubtractedEvent(AccountPointSubtracted accountPointSubtracted)
    {
        var payload = JsonConvert.SerializeObject(accountPointSubtracted);
        var topic = _configuration["MessageQueue:AccountTopic"];
        await _publisher.SendMessage(payload, topic);
    }

    public async Task PublishAccountPointLimitExceededEvent(AccountPointLimitExceeded accountPointLimitExceeded)
    {
        var payload = JsonConvert.SerializeObject(accountPointLimitExceeded);
        var topic = _configuration["MessageQueue:AccountTopic"];
        await _publisher.SendMessage(payload, topic);
    }

    public async Task PublishAccountDebtHasBeenPaidEvent(AccountDebtHasBeenPaid accountDebtHasBeenPaid)
    {
        var payload = JsonConvert.SerializeObject(accountDebtHasBeenPaid);
        var topic = _configuration["MessageQueue:AccountTopic"];
        await _publisher.SendMessage(payload, topic);    
    }    
}
