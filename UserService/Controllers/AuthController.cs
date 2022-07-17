using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.BusinessLogic;
using UserService.Dtos;

namespace UserService.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IUserBusinessLogic _userBusinessLogic;

    public AuthController(IUserBusinessLogic userBusinessLogic)
    {
        _userBusinessLogic = userBusinessLogic;
    }
    
    [HttpPut]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto forgetPasswordDto)
    {
        await _userBusinessLogic.ForgetPassword(forgetPasswordDto);
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> SignUp([FromBody] SignUpDto signUpDto)
    {
        await _userBusinessLogic.SignUp(signUpDto);
        return Ok();
    }
}
