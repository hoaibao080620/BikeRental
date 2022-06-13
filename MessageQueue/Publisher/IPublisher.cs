using Amazon.SimpleNotificationService.Model;

namespace MessageQueue.Publisher;

public interface IPublisher
{
    public Task SendMessage(string message, string topicArn, Dictionary<string, MessageAttributeValue>? messageAttributes);
}