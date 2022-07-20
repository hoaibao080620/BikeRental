using System.Net.Http.Headers;
using System.Security.Claims;
using AccountService.DataAccess;
using AccountService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

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
    [AllowAnonymous]
    public async Task<IActionResult> TestSentry()
    {
        throw new InvalidOperationException();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAccounts()
    {
        var accounts = await _mongoService.FindAccounts(_ => true);
        return Ok(accounts);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllAccountTransactions()
    {
        var accounts = await _mongoService.FindAccountTransactions(_ => true);
        return Ok(accounts);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllAccountPointHistories()
    {
        var accounts = await _mongoService.FindAccountPointHistories(_ => true);
        return Ok(accounts);
    }

    [HttpGet]
    public async Task<IActionResult> GetAccountPaymentHistories([FromQuery] string? email)
    {
        var accountEmail = string.IsNullOrEmpty(email) ? HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value : email;
        var paymentHistories = await _mongoService
            .FindAccountTransactions(x => x.AccountEmail == accountEmail);

        return Ok(paymentHistories);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAccountPointHistories([FromQuery] string? email)
    {
        var accountEmail = string.IsNullOrEmpty(email) ? HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value : email;
        
        var paymentHistories = await _mongoService
            .FindAccountPointHistories(x => x.AccountEmail == accountEmail);

        return Ok(paymentHistories);
    }
    
    [HttpGet]
    public async Task<IActionResult> IsAccountHasEnoughPoint([FromQuery] string accountEmail)
    {
        var account = (await _mongoService.FindAccounts(x => x.Email == accountEmail)).FirstOrDefault();
        return Ok(account?.Point >= 50);
    }
    
    [HttpPut]
    public async Task<IActionResult> LockAccount([FromQuery] string accountId)
    {
        var builder = Builders<Account>.Update
            .Set(x => x.IsActive, false)
            .Set(x => x.UpdatedOn, DateTime.UtcNow);

        await _mongoService.UpdateAccount(accountId, builder);
        return Ok();
    }
    
    [HttpPut]
    public async Task<IActionResult> UnlockAccount([FromQuery] string accountId)
    {
        var builder = Builders<Account>.Update
            .Set(x => x.IsActive, true)
            .Set(x => x.UpdatedOn, DateTime.UtcNow);

        await _mongoService.UpdateAccount(accountId, builder);
        return Ok();
    }
}
