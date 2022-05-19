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
    
    public async Task SendMessage(string message, string topicArn)
    {
        var request = new PublishRequest
        {
            Message = message,
            TopicArn = topicArn
        };

        try
        {
            var response = await _amazonSns.PublishAsync(request);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Caught exception publishing request:");
            Console.WriteLine(ex.Message);
        }
    }
}