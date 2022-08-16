using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.MessageType;
using BikeRental.MessageQueue.Publisher;
using Newtonsoft.Json;
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
            MessageType = MessageType.UserAdded,
            Role = user.RoleName,
            DateOfBirth = user.DateOfBirth,
            Address = user.Address
        };
        
        await _publisher.SendMessage(JsonConvert.SerializeObject(message), _configuration["Topic:UserTopic"]);
    }
    
    public async Task PublishUserUpdatedEventToMessageQueue(User user, string? imageBase64)
    {
        var message = new UserUpdated
        {
            Id = user.Id,
            PhoneNumber = user.PhoneNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MessageType = MessageType.UserUpdated,
            Role = user.RoleName,
            DateOfBirth = user.DateOfBirth,
            Address = user.Address,
            ImageBase64 = imageBase64
        };
        
        await _publisher.SendMessage(
            JsonConvert.SerializeObject(message), 
            _configuration["Topic:UserTopic"]
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
            _configuration["Topic:UserTopic"]);
    }

    public async Task PublishUserRoleUpdatedEvent(UserRoleUpdated userRoleUpdated)
    {
        var topic = _configuration["Topic:UserTopic"];
        var payload = JsonConvert.SerializeObject(userRoleUpdated);

        await _publisher.SendMessage(payload, topic);
    }

    public async Task PublishUserDeactivatedEvent(UserDeactivated userDeactivated)
    {
        var topic = _configuration["Topic:UserTopic"];
        var payload = JsonConvert.SerializeObject(userDeactivated);

        await _publisher.SendMessage(payload, topic);
    }

    public async Task PublishUserActivatedEvent(UserReactivated userReactivated)
    {
        var topic = _configuration["Topic:UserTopic"];
        var payload = JsonConvert.SerializeObject(userReactivated);

        await _publisher.SendMessage(payload, topic);
    }
}
