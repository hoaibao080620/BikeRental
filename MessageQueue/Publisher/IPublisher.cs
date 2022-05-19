namespace MessageQueue.Publisher;

public interface IPublisher
{
    public Task SendMessage(string message, string topicArn);
}