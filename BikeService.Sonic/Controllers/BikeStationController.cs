using System.Security.Claims;
using BikeService.Sonic.BusinessLogics;
using BikeService.Sonic.DAL;
using BikeService.Sonic.Dtos;
using BikeService.Sonic.Dtos.BikeStation;
using BikeService.Sonic.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BikeService.Sonic.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Authorize]
public class BikeStationController : ControllerBase
{
    private readonly IBikeStationBusinessLogic _bikeStationBusinessLogic;
    private readonly IBikeStationValidation _bikeStationValidation;
    private readonly IUnitOfWork _unitOfWork;

    public BikeStationController(IBikeStationBusinessLogic bikeStationBusinessLogic, 
        IBikeStationValidation bikeStationValidation,
        IUnitOfWork unitOfWork)
    {
        _bikeStationBusinessLogic = bikeStationBusinessLogic;
        _bikeStationValidation = bikeStationValidation;
        _unitOfWork = unitOfWork;
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
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
        var bikeStations = (await _bikeStationBusinessLogic.GetAllStationBikes(email))
            .OrderByDescending(x => x.UpdatedOn);
        return Ok(bikeStations);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllAdminBikeStations()
    {
        var bikeStations = (await _bikeStationBusinessLogic.GetAllAdminBikeStation())
            .OrderByDescending(x => x.UpdatedOn);
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
            return BadRequest("Trạm hiện đang có xe, không thể xóa!");
        
        await _bikeStationBusinessLogic.DeleteStationBike(id);
        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetBikeStationColors()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
        
        var bikeStationColors = await _bikeStationBusinessLogic.GetBikeStationColors(email);
        return Ok(bikeStationColors);
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateBikeStationColor([FromBody] List<BikeStationColorDto> bikeStationColors)
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
        
        await _bikeStationBusinessLogic.UpdateBikeStationColor(bikeStationColors, email);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetBikesInStationBike([FromQuery] int bikeStationId)
    {
        var bikes = await _bikeStationBusinessLogic.GetBikeStationBike(bikeStationId);
        return Ok(bikes);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetBikeStationsNearMe([FromQuery] BikeStationRetrieveParameter bikeStationRetrieveParameter)
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
        var bikeStations = await _bikeStationBusinessLogic
            .GetBikeStationsNearMe(bikeStationRetrieveParameter, email);
        return Ok(bikeStations);
    }

    [HttpPut]
    public async Task<IActionResult> AssignBikesToStation([FromBody] BikeStationBikeAssignDto bikeAssignDto)
    {
        var errorMessage = await _bikeStationValidation.IsAssignBikesValid(bikeAssignDto.BikeIds, bikeAssignDto.BikeStationId);
        if (errorMessage is not null) return BadRequest(errorMessage);
        await _bikeStationBusinessLogic.AssignBikesToBikeStation(bikeAssignDto);

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAssignableBikeStation([FromQuery] int totalBikeAssign)
    {
        var bikeStations = await _bikeStationBusinessLogic.GetAssignableBikeStations(totalBikeAssign);
        return Ok(bikeStations);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAssignableManager()
    {
        var managers = await _bikeStationBusinessLogic.GetAssignableManagers();
        return Ok(managers);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAssignableStations()
    {
        var managers = await _bikeStationBusinessLogic.GetAssignableManagers();
        return Ok(managers);
    }
    
    [HttpPut]
    public async Task<IActionResult> AssignBikeStationsToManager([FromBody] BikeStationManagerAssignDto bikeAssignDto)
    {
        await _bikeStationBusinessLogic.AssignBikeStationsToManager(bikeAssignDto);
        return Ok();
    }
    
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GenerateData()
    {
        var bikeStations = await _unitOfWork.BikeStationRepository.Find(x => x.Code == "<null>");
        foreach (var bikeStation in bikeStations.ToList())
        {
            bikeStation.Code = $"BS-{bikeStation.Id.ToString().PadLeft(6, '0')}";
        }

        await _unitOfWork.SaveChangesAsync();
        return Ok();
    }
}
