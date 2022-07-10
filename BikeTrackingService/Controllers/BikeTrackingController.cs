using System.Security.Claims;
using BikeTrackingService.BLL;
using BikeTrackingService.Dtos.BikeOperation;
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
    
    [HttpGet]
    public async Task<IActionResult> GetBikesTracking()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;

        var histories = await _bikeTrackingBusinessLogic.GetBikesTracking(email);
        return Ok(histories);
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateBikeLocation(BikeLocationDto bikeLocationDto)
    {
        await _bikeTrackingBusinessLogic.UpdateBikeLocation(bikeLocationDto);
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> Checking(BikeCheckinDto bikeCheckinDto)
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
        
        await _bikeTrackingBusinessLogic.BikeChecking(bikeCheckinDto, email);
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> Checkout(BikeCheckoutDto bikeCheckoutDto)
    {
        if (bikeCheckoutDto.BikeStationId is null)
            return BadRequest("You have to scan QR code of station before scan bike QR code");
        
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
        
        await _bikeTrackingBusinessLogic.BikeCheckout(bikeCheckoutDto, email);
        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetRentingStatus()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
        var rentingStatus = await _bikeTrackingBusinessLogic.GetBikeRentingStatus(email);
    
        return Ok(rentingStatus);
    }
}
