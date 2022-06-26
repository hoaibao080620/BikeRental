using Amazon.SimpleNotificationService.Model;

namespace BikeRental.MessageQueue.Publisher;

public interface IPublisher
{
    public Task SendMessage(string message, string topicArn, Dictionary<string, MessageAttributeValue>? messageAttributes = null);
}
