using BikeService.Sonic.BusinessLogics;
using BikeService.Sonic.Dtos;
using BikeService.Sonic.Dtos.Bike;
using Microsoft.AspNetCore.Mvc;

namespace BikeService.Sonic.Controllers;

[ApiController]
[Route("[controller]")]
// [Route("[controller]")]
// [Authorize]
public class BikeController : ControllerBase
{
    private readonly IBikeBusinessLogic _bikeBusinessLogic;

    public BikeController(IBikeBusinessLogic bikeBusinessLogic)
    {
        _bikeBusinessLogic = bikeBusinessLogic;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetBikes()
    {
        var bikes = await _bikeBusinessLogic.GetBikes();
        return Ok(bikes);
    }
    
    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> GetBike(int id)
    {
        var bike = await _bikeBusinessLogic.GetBike(id);
        return Ok(bike);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBike(BikeInsertDto bikeInsertDto)
    {
        await _bikeBusinessLogic.AddBike(bikeInsertDto);

        return Ok(bikeInsertDto);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateBike(BikeUpdateDto bikeUpdateDto)
    {
        await _bikeBusinessLogic.UpdateBike(bikeUpdateDto);

        return Ok();
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteBike(int id)
    {
        await _bikeBusinessLogic.DeleteBike(id);

        return Ok();
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> UpdateBikeLocation(BikeLocationDto bikeLocationDto)
    {
        await _bikeBusinessLogic.UpdateBikeLocation(bikeLocationDto);
        return Ok();
    }
    
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Checking(BikeCheckingDto bikeCheckingDto)
    {
        await _bikeBusinessLogic.BikeChecking(bikeCheckingDto);
        return Ok();
    }
    
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> Checkout(BikeCheckoutDto bikeCheckoutDto)
    {
        await _bikeBusinessLogic.BikeCheckout(bikeCheckoutDto);
        return Ok();
    }
}