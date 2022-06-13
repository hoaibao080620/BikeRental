using Amazon.SQS.Model;

namespace BikeRental.MessageQueue.Consumer;

public interface IConsumer
{
    public Task<List<Message>> ReceiveMessages(string queue);
}