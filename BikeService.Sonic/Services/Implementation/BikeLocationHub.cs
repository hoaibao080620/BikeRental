using BikeService.Sonic.Const;
using BikeService.Sonic.Models;
using BikeService.Sonic.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace BikeService.Sonic.Services.Implementation;

public class BikeLocationHub : Hub, IBikeLocationHub
{
    private readonly IHubContext<BikeLocationHub> _hubContext;

    public BikeLocationHub(IHubContext<BikeLocationHub> hubContext)
    {
        _hubContext = hubContext;
    }
    
    public async Task SendBikeLocationsData(List<Bike> bikes)
    {
        await _hubContext.Clients.All.SendAsync(SignalRChannel.BikeLocationChannel, bikes);
    }
}