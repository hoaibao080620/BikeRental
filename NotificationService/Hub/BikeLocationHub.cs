using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Consts;
using Okta.AspNetCore;

namespace NotificationService.Hub;

[Authorize(AuthenticationSchemes = OktaDefaults.ApiAuthenticationScheme)]
public class BikeLocationHub : Microsoft.AspNetCore.SignalR.Hub, IBikeLocationHub
{
    private readonly IHubContext<BikeLocationHub> _hubContext;

    public BikeLocationHub(IHubContext<BikeLocationHub> hubContext)
    {
        _hubContext = hubContext;
    }
    
    public async Task NotifyBikeLocationHasChanged(string? email)
    {
        if (string.IsNullOrEmpty(email)) return;
        await _hubContext.Clients.Group(email).SendAsync(SignalRChannel.BikeLocationChangeChannel);
    }
    
    public override async Task OnConnectedAsync()
    {
        var email = Context.GetHttpContext()!.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;

        await Groups.AddToGroupAsync(Context.ConnectionId, email);
    }
}
