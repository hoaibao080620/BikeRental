using Amazon.SQS.Model;

namespace MessageQueue.Consumer;

public interface IConsumer
{
    public Task<List<Message>> ReceiveMessages(string queue);
}