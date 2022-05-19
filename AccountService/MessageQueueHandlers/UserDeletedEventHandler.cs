﻿using AccountService.DataAccess.Interfaces;
using MessageQueue.Events;
using MessageQueue.Handlers;
using Newtonsoft.Json;

namespace AccountService.MessageQueueHandlers;

public class UserDeletedEventHandler : IMessageQueueHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public UserDeletedEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task Handle(string message)
    {
        var userDeleted = JsonConvert.DeserializeObject<UserDeleted>(message);

        if(userDeleted is null) return;

        var user = (await _unitOfWork.UserRepository
            .Find(x => x.ExternalId == userDeleted.UserId)).FirstOrDefault();

        if (user is null) return;
        
        var account = (await _unitOfWork.AccountRepository
            .Find(x => x.UserId == user.Id)).FirstOrDefault();
        
        if (account is null) return;
        
        await _unitOfWork.AccountRepository.Delete(account);
        await _unitOfWork.UserRepository.Delete(user);
        
        await _unitOfWork.SaveChangesAsync();
    }
}