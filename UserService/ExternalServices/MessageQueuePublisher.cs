using Amazon.SimpleNotificationService.Model;
using MessageQueue.Events;
using MessageQueue.Publisher;
using Newtonsoft.Json;
using Shared.Consts;
using UserService.Models;

namespace UserService.ExternalServices;

public class MessageQueuePublisher : IMessageQueuePublisher
{
    private readonly IPublisher _publisher;
    private readonly IConfiguration _configuration;

    public MessageQueuePublisher(IPublisher publisher, IConfiguration configuration)
    {
        _publisher = publisher;
        _configuration = configuration;
    }
    
    public async Task PublishUserAddedEventToMessageQueue(User user)
    {
        var message = new UserCreated
        {
            Id = user.Id,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MessageType = MessageType.UserAdded
        };
        
        await _publisher.SendMessage(
            JsonConvert.SerializeObject(message), 
            _configuration["Topic:UserTopic"], 
            GetUserGroupMessageAttributeValues(user.Role.Name));
    }
    
    public async Task PublishUserUpdatedEventToMessageQueue(User user)
    {
        var message = new UserUpdated
        {
            Id = user.Id,
            PhoneNumber = user.PhoneNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MessageType = MessageType.UserUpdated
        };
        
        await _publisher.SendMessage(
            JsonConvert.SerializeObject(message), 
            _configuration["Topic:UserTopic"], 
            GetUserGroupMessageAttributeValues(user.Role.Name)
            );
    }
    
    public async Task PublishUserDeletedEventToMessageQueue(User user)
    {
        var message = new UserDeleted
        {
            UserId = user.Id,
            MessageType = MessageType.UserDeleted
        };
        
        await _publisher.SendMessage(
            JsonConvert.SerializeObject(message), 
            _configuration["Topic:UserTopic"],
            GetUserGroupMessageAttributeValues(user.Role.Name));
    }

    private static Dictionary<string, MessageAttributeValue> GetUserGroupMessageAttributeValues(string group)
    {
        return new Dictionary<string, MessageAttributeValue>
        {
            {
                "user_group", new MessageAttributeValue
                {
                    DataType = "String",
                    StringValue = group
                }
            }
        };
    }
}