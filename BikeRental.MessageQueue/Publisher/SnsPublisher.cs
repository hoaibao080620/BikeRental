using Amazon;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace BikeRental.MessageQueue.Publisher;

public class SnsPublisher : IPublisher
{
    private readonly IAmazonSimpleNotificationService _amazonSns; 
    public SnsPublisher()
    {
        var basicCredentials = new BasicAWSCredentials("AKIA2JUZUHJX2ACUJK7R", 
            "QLWvm1G1dJ0DCNPcet6DelFMmqbKVcz2WTq4Qw8x");
        _amazonSns = new AmazonSimpleNotificationServiceClient(basicCredentials, RegionEndpoint.USEast1);
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
