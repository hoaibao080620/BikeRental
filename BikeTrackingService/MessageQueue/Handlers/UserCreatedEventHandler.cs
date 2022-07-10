﻿using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using BikeTrackingService.DAL;
using BikeTrackingService.Models;
using Newtonsoft.Json;
using Shared.Consts;

namespace BikeTrackingService.MessageQueue.Handlers;

public class UserCreatedEventHandler : IMessageQueueHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public UserCreatedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task Handle(string message)
    {
        var userCreatedMessage = JsonConvert.DeserializeObject<UserCreated>(message);
        if(userCreatedMessage?.Role != UserRole.User) return;
        
        await _unitOfWork.AccountRepository.Add(new Account
        {
            CreatedOn = DateTime.UtcNow,
            IsActive = true,
            Email = userCreatedMessage.Email,
            PhoneNumber = userCreatedMessage.PhoneNumber!,
            ExternalId = userCreatedMessage.Id
        });
        await _unitOfWork.SaveChangesAsync();
    }
}
