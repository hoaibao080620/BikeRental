using System.Security.Claims;
using BikeService.Sonic.BusinessLogics;
using BikeService.Sonic.Dtos;
using BikeService.Sonic.Dtos.BikeStation;
using BikeService.Sonic.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace BikeService.Sonic.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class BikeStationController : ControllerBase
{
    private readonly IBikeStationBusinessLogic _bikeStationBusinessLogic;
    private readonly IBikeStationValidation _bikeStationValidation;

    public BikeStationController(IBikeStationBusinessLogic bikeStationBusinessLogic, IBikeStationValidation bikeStationValidation)
    {
        _bikeStationBusinessLogic = bikeStationBusinessLogic;
        _bikeStationValidation = bikeStationValidation;
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> GetBikeStation(int id)
    {
        var bikeStation = await _bikeStationBusinessLogic.GetStationBike(id);
        return Ok(bikeStation);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllBikeStations()
    {
        var bikeStations = await _bikeStationBusinessLogic.GetAllStationBikes();
        return Ok(bikeStations);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddStationBike(BikeStationInsertDto stationInsertDto)
    {
        await _bikeStationBusinessLogic.AddStationBike(stationInsertDto);
        return Ok(stationInsertDto);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateStationBike(BikeStationUpdateDto bikeStationUpdateDto)
    {
        await _bikeStationBusinessLogic.UpdateStationBike(bikeStationUpdateDto);

        return Ok();
    }
    
    [HttpDelete]
    [Route("{id:int}")]
    public async Task<IActionResult> DeleteStationBike(int id)
    {
        if (await _bikeStationValidation.IsBikeStationHasBikes(id)) 
            return BadRequest("Bike station has bike, cannot delete it!");
        
        await _bikeStationBusinessLogic.DeleteStationBike(id);
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateBikeStationColor([FromBody] BikeStationColorDto bikeStationColorDto)
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
        await _bikeStationBusinessLogic.UpdateBikeStationColor(bikeStationColorDto, email);
        return Ok();
    }
}
