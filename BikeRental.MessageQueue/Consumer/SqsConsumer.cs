﻿using System.Net;
using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace BikeRental.MessageQueue.Consumer;

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

        return result.Messages;
    }

    public async Task DeleteMessage(string queue, Message message)
    {
        await _amazonSqs.DeleteMessageAsync(queue, message.ReceiptHandle);
    }
}
