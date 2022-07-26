using Amazon.SQS.Model;

namespace BikeRental.MessageQueue.Consumer;

public interface IConsumer
{
    Task<List<Message>> ReceiveMessages(string queue);
    Task DeleteMessage(string queue, Message message);
}
