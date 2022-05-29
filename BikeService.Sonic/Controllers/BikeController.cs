using BikeService.Sonic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BikeService.Sonic.Controllers;

[ApiController]
[Route("[controller]")]
public class BikeController : ControllerBase
{
    private readonly IBikeLocationHub _bikeLocationHub;

    public BikeController(IBikeLocationHub bikeLocationHub)
    {
        _bikeLocationHub = bikeLocationHub;
    }
    
    
}