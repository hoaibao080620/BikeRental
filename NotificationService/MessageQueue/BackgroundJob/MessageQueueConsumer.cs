﻿using BikeRental.MessageQueue.Consumer;
using BikeRental.MessageQueue.SubscriptionManager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NotificationService.MessageQueue.BackgroundJob;

public class MessageQueueConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly IMessageQueueSubscriptionManager _messageQueueSubscriptionManager;
    private readonly ILogger<MessageQueueConsumer> _logger;

    public MessageQueueConsumer(
        IServiceProvider serviceProvider, 
        IConfiguration configuration,
        IMessageQueueSubscriptionManager messageQueueSubscriptionManager,
        ILogger<MessageQueueConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _messageQueueSubscriptionManager = messageQueueSubscriptionManager;
        _logger = logger;
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
            var receiveMessages = await consumer.ReceiveMessages(_configuration["MessageQueue:NotificationQueue"]);
            foreach (var message in receiveMessages)
            {
                var messageType = (JsonConvert.DeserializeObject(message.Body) as JObject)?["MessageType"]?.ToString();
                if(messageType is null) continue;
                
                try
                {
                    var messageHandler = _messageQueueSubscriptionManager.GetHandler(messageType);
                    await messageHandler.Handle(message.Body);
                }
                catch (InvalidOperationException exception)
                {
                    _logger.LogError(exception.Message);
                }
            }
        }
    }
}