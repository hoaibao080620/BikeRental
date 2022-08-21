using System.Security.Claims;
using BikeService.Sonic.BusinessLogics;
using BikeService.Sonic.Dtos;
using BikeService.Sonic.Dtos.Bike;
using Bogus;
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
    [Route("[action]")]
    public async Task<IActionResult> GetAllBikes()
    {
        var bikes = await _bikeBusinessLogic.GetAllBikes();
        return Ok(bikes);
    }

    [HttpGet]
    [Route("{bikeCode}")]
    public async Task<IActionResult> GetBike(string bikeCode)
    {
        var bike = await _bikeBusinessLogic.GetBike(bikeCode);
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
    
    [HttpPut]
    [Route("[action]")]
    public async Task<IActionResult> UnlockBike([FromBody] BikeUnlockDto bikeUnlockDto)
    {
        await _bikeBusinessLogic.UnlockBike(bikeUnlockDto.BikeId);
        return Ok();
    }
    
    [HttpGet]
    [Route("[action]")]
    [AllowAnonymous]
    public async Task<IActionResult> GenerateData(int? numberOfItems = 0)
    {
        var bikeModel = new List<string>
        {
            "Merida",
            "Trek",
            "Giant",
            "Kona",
            "Marin",
            "GT",
            "Jek",
            "Trinx"
        };
        
        var faker = new Faker("vi");
        for (var i = 0; i < numberOfItems; i++)
        {
            var bike = new BikeInsertDto
            {
                Description = $"{faker.PickRandom(bikeModel)} {faker.Vehicle.Model()}"
            };

            await _bikeBusinessLogic.AddBike(bike);
        }

        return Ok();
    }
}
