using BikeRental.MessageQueue.Consumer;
using BikeRental.MessageQueue.Handlers;
using BikeRental.MessageQueue.SubscriptionManager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AccountService.BackgroundJob;

public class MessageQueueConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly IMessageQueueSubscriptionManager _messageQueueSubscriptionManager;

    public MessageQueueConsumer(
        IServiceProvider serviceProvider, 
        IConfiguration configuration,
        IMessageQueueSubscriptionManager messageQueueSubscriptionManager)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _messageQueueSubscriptionManager = messageQueueSubscriptionManager;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConsumeMessage(stoppingToken);
    }

    private async Task ConsumeMessage(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var consumer = scope.ServiceProvider.GetRequiredService<IConsumer>();

        while (!cancellationToken.IsCancellationRequested)
        {
            var receiveMessages = await consumer.ReceiveMessages(_configuration["MessageQueue:AccountQueue"]);
            foreach (var message in receiveMessages)
            {
                var messageType = (JsonConvert.DeserializeObject(message.Body) as JObject)?["MessageType"]?.ToString();
                
                if(messageType is null) continue;
                
                var messageHandlerType = _messageQueueSubscriptionManager.GetHandler(messageType ?? string.Empty);
                var messageHandler = (IMessageQueueHandler) ActivatorUtilities.CreateInstance(scope.ServiceProvider, messageHandlerType);
                await messageHandler.Handle(message.Body);
                await consumer.DeleteMessage(_configuration["MessageQueue:AccountQueue"], message);
            }
        }
    }
}
