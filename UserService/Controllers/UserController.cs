using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.BusinessLogic;

namespace UserService.Controllers;

[Route("[controller]/[action]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserBusinessLogic _userBusinessLogic;
    private readonly IMapper _mapper;

    public UserController(IUserBusinessLogic userBusinessLogic, IMapper mapper)
    {
        _userBusinessLogic = userBusinessLogic;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetUserProfile()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
        var user = await _userBusinessLogic.GetUserProfile(email);
        return Ok(user);
    }

    // [HttpGet]
    // public async Task<IActionResult> GetUsers()
    // {
    //     var users = await _userBusinessLogic.GetUsers();
    //     return Ok(users);
    // }
    //
    // [HttpGet("{userId:int}")]
    // public async Task<IActionResult> GetUser(int userId)
    // {
    //     var user = await _userBusinessLogic.GetUserById(userId);
    //     return Ok(_mapper.Map<UserRetrieveDto>(user));
    // }
    //
    // [HttpPost]
    // public async Task<IActionResult> AddUser([FromBody] UserInsertDto userInsertDto)
    // {
    //     await _userBusinessLogic.AddUser(userInsertDto);
    //     return Ok();
    // }
    //
    // [HttpPut("{userId:int}")]
    // public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserUpdateDto userUpdateDto)
    // {
    //     await _userBusinessLogic.UpdateUser(userId, userUpdateDto);
    //     return Ok();
    // }
    //
    // [HttpDelete("{userId:int}")]
    // public async Task<IActionResult> DeleteUser(int userId)
    // {
    //     await _userBusinessLogic.DeleteUser(userId);
    //     return Ok();
    // }
}
