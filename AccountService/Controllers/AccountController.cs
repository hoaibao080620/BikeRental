using System.Security.Claims;
using AccountService.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly IMongoService _mongoService;

    public AccountController(IMongoService mongoService)
    {
        _mongoService = mongoService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAccountPaymentHistories()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
        var paymentHistories = await _mongoService
            .FindAccountTransactions(x => x.AccountEmail == email);

        return Ok(paymentHistories);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAccountPointHistories()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
        var paymentHistories = await _mongoService
            .FindAccountPointHistories(x => x.AccountEmail == email);

        return Ok(paymentHistories);
    }
}
