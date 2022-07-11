using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.BusinessLogic;
using UserService.Dtos;

namespace UserService.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserBusinessLogic _userBusinessLogic;

    public UserController(IUserBusinessLogic userBusinessLogic)
    {
        _userBusinessLogic = userBusinessLogic;
    }
    
    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> GetUserProfile()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
        var user = await _userBusinessLogic.GetUserProfile(email);
        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
        var users = await _userBusinessLogic.GetUsers(email);
        return Ok(users);
    }
    
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(string userId)
    {
        var users = await _userBusinessLogic.GetUserById(userId);
        return Ok(users);
    }
    
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> AddUser([FromBody] UserInsertDto userInsertDto)
    {
        await _userBusinessLogic.AddUser(userInsertDto);
        return Ok();
    }
    
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] UserUpdateDto userUpdateDto)
    {
        await _userBusinessLogic.UpdateUser(userId, userUpdateDto);
        return Ok();
    }
    
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        await _userBusinessLogic.DeleteUser(userId);
        return Ok();
    }
}
