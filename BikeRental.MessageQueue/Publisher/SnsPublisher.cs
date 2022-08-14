﻿using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace BikeRental.MessageQueue.Publisher;

public class SnsPublisher : IPublisher
{
    private readonly IAmazonSimpleNotificationService _amazonSns; 
    public SnsPublisher()
    {
        _amazonSns = new AmazonSimpleNotificationServiceClient(RegionEndpoint.USEast1);
    }
    
    public async Task SendMessage(string message, string topicArn, Dictionary<string, MessageAttributeValue>? messageAttributes = null)
    {
        var request = new PublishRequest
        {
            Message = message,
            TopicArn = topicArn,
            MessageAttributes = messageAttributes
        };
        
        await _amazonSns.PublishAsync(request);
    }
}
