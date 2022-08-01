using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.DAL;
using NotificationService.Dto;

namespace NotificationService.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationController(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
        var notifications = await _notificationRepository.GetNotifications(email);
        return Ok(notifications);
    }
    
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> MarkNotificationSeen()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
        await _notificationRepository.MarkNotificationSeen(email);
        return Ok();
    }
    
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> MarkNotificationOpen([FromBody] MarkNotificationOpenDto markNotificationOpenDto)
    {
        await _notificationRepository.MarkNotificationOpen(markNotificationOpenDto.Id);
        return Ok();
    }
    
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> CreateNotification([FromBody] MarkNotificationOpenDto markNotificationOpenDto)
    {
        await _notificationRepository.MarkNotificationOpen(markNotificationOpenDto.Id);
        return Ok();
    }
}
