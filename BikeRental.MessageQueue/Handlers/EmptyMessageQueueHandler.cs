namespace BikeRental.MessageQueue.Handlers;

public class EmptyMessageQueueHandler : IMessageQueueHandler
{
    public Task Handle(string message)
    {
        // Do nothing
        return Task.CompletedTask;
    }
}
