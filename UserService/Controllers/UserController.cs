using System.Security.Claims;
using Bogus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Consts;
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
    
    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> GetManagers()
    {
        var users = await _userBusinessLogic.GetManagers();
        return Ok(users);
    }
    
    [HttpPut]
    [Route("[action]")]
    public async Task<IActionResult> DeactivateUser([FromBody] ActivateUserDto activateUserDto)
    {
        await _userBusinessLogic.DeactivateUser(activateUserDto);
        return Ok();
    }
    
    [HttpPut]
    [Route("[action]")]
    public async Task<IActionResult> DeactivateMultipleUser([FromBody] List<string> ids)
    {
        foreach (var id in ids)
        {
            await _userBusinessLogic.DeactivateUser(new ActivateUserDto
            {
                UserId = id
            });
        }
        return Ok();
    }
    
    [HttpPut]
    [Route("[action]")]
    public async Task<IActionResult> DeleteMultipleUser([FromBody] List<string> ids)
    {
        foreach (var id in ids)
        {
            await _userBusinessLogic.DeleteUser(id);
        }
        return Ok();
    }
    
    [HttpPut]
    [Route("[action]")]
    public async Task<IActionResult> ActivateUser([FromBody] ActivateUserDto activateUserDto)
    {
        await _userBusinessLogic.ActivateUser(activateUserDto);
        return Ok();
    }
    
    [HttpGet]
    [Route("[action]")]
    [AllowAnonymous]
    public async Task<IActionResult> GenerateData(int? numberOfItems = 0)
    {
        var roles = new List<string>
        {
            UserRole.User,
            UserRole.Manager,
            UserRole.Director,
            UserRole.SysAdmin
        };
        
        var faker = new Faker("vi");
        var users = new List<UserInsertDto>();
        for (var i = 0; i < numberOfItems; i++)
        {
            var firstName = faker.Name.FirstName();
            var lastName = faker.Name.LastName();
            var user = new UserInsertDto
            {
                FirstName = firstName,
                LastName = lastName,
                Address = $"{faker.Address.StreetName()} - {faker.Address.City()}",
                DateOfBirth = faker.Date.Between(new DateTime(1990 ,1 ,1), new DateTime(2006,1, 1)),
                Email = faker.Internet.Email(firstName, lastName, "gmail.com"),
                Password = "RinRin123^^!",
                PhoneNumber = faker.Phone.PhoneNumber("+84#########"),
                RoleName = faker.PickRandom(roles)
            };
            users.Add(user);

            if (user.RoleName == UserRole.User)
            {
                await _userBusinessLogic.SignUp(new SignUpDto
                {
                    Address = user.Address,
                    DateOfBirth = user.DateOfBirth,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Password = user.Password,
                    PhoneNumber = user.PhoneNumber
                });
            }
            else
            {
                await _userBusinessLogic.AddUser(user);
            }
        }
        
        return Ok(users);
    }
}
