using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace MessageQueue.Publisher;

public class SqsPublisher : IPublisher
{
    private readonly IAmazonSimpleNotificationService _amazonSns; 
    public SqsPublisher()
    {
        var basicCredentials = new BasicAWSCredentials("AKIA2JUZUHJXQBX37EUR", 
            "bP0ROTQSIv8nJr1P+OZ91duCzOhElC9ud2qG2db0");
        
        _amazonSns = new AmazonSimpleNotificationServiceClient(basicCredentials, RegionEndpoint.USEast1);
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