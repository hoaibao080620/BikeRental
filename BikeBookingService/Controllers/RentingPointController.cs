using System.Security.Claims;
using BikeBookingService.DAL;
using BikeBookingService.Dtos;
using BikeBookingService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BikeBookingService.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Authorize]
public class RentingPointController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public RentingPointController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCurrentRentingPoint()
    {
        var currentRentingPoint = (await _unitOfWork.RentingPointRepository.All()).FirstOrDefault();
        return Ok(currentRentingPoint);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetRentingPointHistory()
    {
        var rentingPointHistory = await _unitOfWork.RentingPointHistoryRepository.All();
        return Ok(rentingPointHistory);
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateRentingPoint([FromBody] RentingPointUpdateDto rentingPointUpdateDto)
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
        var currentRentingPoint = (await _unitOfWork.RentingPointRepository.All()).FirstOrDefault();
        if (currentRentingPoint is not null)
        {
            await _unitOfWork.RentingPointRepository.Delete(currentRentingPoint);
        }

        await _unitOfWork.RentingPointRepository.Add(new RentingPoint
        {
            PointPerHour = rentingPointUpdateDto.Point,
            CreatedOn = DateTime.UtcNow
        });
        
        await _unitOfWork.RentingPointHistoryRepository.Add(new RentingPointHistory()
        {
            PointChange = rentingPointUpdateDto.Point,
            CreatedOn = DateTime.UtcNow,
            ChangeReason = rentingPointUpdateDto.ChangeReason,
            ChangeBy = email
        });

        await _unitOfWork.SaveChangesAsync();
        return Ok();
    }
}
