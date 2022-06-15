using BikeService.Sonic.BusinessLogics;
using BikeService.Sonic.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace BikeService.Sonic.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Authorize]
public class BikeStationController : ControllerBase
{
    private readonly IBikeStationBusinessLogic _bikeStationBusinessLogic;
    public BikeStationController(IBikeStationBusinessLogic bikeStationBusinessLogic)
    {
        _bikeStationBusinessLogic = bikeStationBusinessLogic;
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
    [Route("[action]")]
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
    public async Task<IActionResult> DeleteStationBike(int id)
    {
        await _bikeStationBusinessLogic.DeleteStationBike(id);

        return Ok();
    }
}