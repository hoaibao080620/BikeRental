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
        
        await _publisher.SendMessage(JsonConvert.SerializeObject(message), _configuration["Topic:UserTopic"]);
    }
    
    public async Task PublishUserUpdatedEventToMessageQueue(User user)
    {
        var message = new UserUpdated()
        {
            Id = user.Id,
            PhoneNumber = user.PhoneNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MessageType = MessageType.UserUpdated
        };
        
        await _publisher.SendMessage(JsonConvert.SerializeObject(message), _configuration["Topic:UserTopic"]);
    }
    
    public async Task PublishUserDeletedEventToMessageQueue(int userId)
    {
        var message = new UserDeleted
        {
            UserId = userId,
            MessageType = MessageType.UserDeleted
        };
        
        await _publisher.SendMessage(JsonConvert.SerializeObject(message), _configuration["Topic:UserTopic"]);
    }
}