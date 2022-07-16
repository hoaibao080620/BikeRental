﻿using System.Security.Claims;
using AccountService.DataAccess;
using AccountService.Dto;
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
    public async Task<IActionResult> GetAllAccounts()
    {
        var accounts = await _mongoService.FindAccounts(_ => true);
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
    
    [HttpPut]
    public async Task<IActionResult> TryToMinusPoint([FromBody] AccountPointSubtract accountPointSubtract)
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;

        var account = (await _mongoService.FindAccounts(x => x.Email == email)).FirstOrDefault();

        if (account is null) return NotFound();

        if (account.Point < accountPointSubtract.Point) return BadRequest("You does not have enough point!");
        
        var builder = Builders<Account>.Update
            .Set(x => x.Point, account.Point - accountPointSubtract.Point);

        await _mongoService.UpdateAccount(account.Id, builder);

        return Ok();
    }
}