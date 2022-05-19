using System.Net;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace MessageQueue.Consumer;

public class SqsConsumer : IConsumer
{
    private readonly AmazonSQSClient _amazonSqs;

    public SqsConsumer()
    {
        _amazonSqs = new AmazonSQSClient(RegionEndpoint.USEast1);
    }
    
    public async Task<List<Message>> ReceiveMessages(string queue)
    {
        var request = new ReceiveMessageRequest  
        {  
            QueueUrl = queue,  
            MaxNumberOfMessages = 10,  
            WaitTimeSeconds = 5  
        };
        
        var result = await _amazonSqs.ReceiveMessageAsync(request);
        if (result.HttpStatusCode != HttpStatusCode.OK) throw new Exception("Error when pull messages from queue");

        if (result.Messages.Any())
        {
            await _amazonSqs.DeleteMessageBatchAsync(queue, result.Messages.Select(x => 
                new DeleteMessageBatchRequestEntry
                {
                    Id = x.MessageId,
                    ReceiptHandle = x.ReceiptHandle
                }).ToList());
        }
        
        return result.Messages;
    }
}