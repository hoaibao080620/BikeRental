using System.Security.Claims;
using BikeService.Sonic.BusinessLogics;
using BikeService.Sonic.Dtos.Bike;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BikeService.Sonic.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class BikeReportController : ControllerBase
{
    private readonly IBikeReportBusinessLogic _bikeReportBusinessLogic;

    public BikeReportController(IBikeReportBusinessLogic bikeReportBusinessLogic)
    {
        _bikeReportBusinessLogic = bikeReportBusinessLogic;
    }

    [HttpGet]
    public async Task<IActionResult> GetBikeReports()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;

        var reports = await _bikeReportBusinessLogic.GetBikeReports(email);
        return Ok(reports);
    }

    [HttpPost]
    public async Task<IActionResult> CreateReport([FromBody] BikeReportInsertDto bikeReportInsertDto)
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;

        await _bikeReportBusinessLogic.CreateReport(bikeReportInsertDto, email);
        return Ok();
    }
}
