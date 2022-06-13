using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace MessageQueue.Publisher;

public class SqsPublisher : IPublisher
{
    private readonly IAmazonSimpleNotificationService _amazonSns; 
    public SqsPublisher()
    {
        _amazonSns = new AmazonSimpleNotificationServiceClient(RegionEndpoint.USEast1);
    }
    
    public async Task SendMessage(string message, string topicArn, Dictionary<string, MessageAttributeValue>? messageAttributes)
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