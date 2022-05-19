namespace MessageQueue.Handlers;

public interface IMessageQueueHandler
{
    Task Handle(string message);
}