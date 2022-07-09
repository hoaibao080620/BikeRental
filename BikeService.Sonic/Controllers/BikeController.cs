using System.Security.Claims;
using BikeService.Sonic.BusinessLogics;
using BikeService.Sonic.Dtos.Bike;
using BikeService.Sonic.Dtos.BikeOperation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Service;

namespace BikeService.Sonic.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class BikeController : ControllerBase
{
    private readonly IBikeBusinessLogic _bikeBusinessLogic;
    private readonly IImportService _importService;

    public BikeController(IBikeBusinessLogic bikeBusinessLogic, IImportService importService)
    {
        _bikeBusinessLogic = bikeBusinessLogic;
        _importService = importService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetBikes()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
        
        var bikes = await _bikeBusinessLogic.GetBikes(email);
        return Ok(bikes);
    }
    
    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> GetBike(int id)
    {
        var bike = await _bikeBusinessLogic.GetBike(id);
        if (bike is null) return NotFound();
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
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBike(int id)
    {
        await _bikeBusinessLogic.DeleteBike(id);
        return Ok();
    }

    
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> ImportBikes()
    {
        if (!Request.Form.Files.Any()) return BadRequest("No files upload");
        
        await _importService.Import(Request.Form.Files[0]);
        return Ok();
    }

    // [HttpGet]
    // [Route("[action]")]
    // public async Task<IActionResult> GetRentingStatus()
    // {
    //     var email = HttpContext.User.Claims.FirstOrDefault(x => 
    //         x.Type == ClaimTypes.NameIdentifier)!.Value;
    //     var rentingStatus = await _bikeBusinessLogic.GetBikeRentingStatus(email);
    //
    //     return Ok(rentingStatus);
    // }
    
    [HttpDelete]
    [Route("[action]")]
    public async Task<IActionResult> DeleteBikes([FromQuery] string bikeIds)
    {
        try
        {
            await _bikeBusinessLogic.DeleteBikes(bikeIds.Split(",").Select(int.Parse).ToList());
            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}
