using System.Security.Claims;
using BikeTrackingService.BLL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BikeTrackingService.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Authorize]
public class BikeTrackingController : ControllerBase
{
    private readonly IBikeTrackingBusinessLogic _bikeTrackingBusinessLogic;

    public BikeTrackingController(IBikeTrackingBusinessLogic bikeTrackingBusinessLogic)
    {
        _bikeTrackingBusinessLogic = bikeTrackingBusinessLogic;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetBikeRentingHistory()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;

        var histories = await _bikeTrackingBusinessLogic.GetBikeRentingHistories(email);
        return Ok(histories);
    }
}
