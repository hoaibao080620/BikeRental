﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Consts;
using NotificationService.Models;

namespace NotificationService.Hubs;

[Authorize]
public class NotificationHub : Hub, INotificationHub
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationHub(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }
    
    public async Task NotifyBikeLocationHasChanged(string? email)
    {
        if (string.IsNullOrEmpty(email)) return;
        await _hubContext.Clients.Group(email).SendAsync(SignalRChannel.BikeLocationChangeChannel);
    }

    public async Task PushNotification(string? email, Notification notification)
    {
        if (string.IsNullOrEmpty(email)) return;
        await _hubContext.Clients.Group(email).SendAsync(SignalRChannel.NotificationChannel, notification);
    }

    public async Task PushPointRunOutNotification(string? email, string message)
    {
        if (string.IsNullOrEmpty(email)) return;
        await _hubContext.Clients.Group(email).SendAsync(SignalRChannel.WarningUserChannel, message);
    }

    public async Task PushUserAnswerPhoneCall(string email)
    {
        if (string.IsNullOrEmpty(email)) return;
        await _hubContext.Clients.Group(email).SendAsync(SignalRChannel.UserAnswerPhoneCall);
    }

    public override async Task OnConnectedAsync()
    {
        var email = Context.GetHttpContext()!.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
    
        await Groups.AddToGroupAsync(Context.ConnectionId, email);
    }
}
