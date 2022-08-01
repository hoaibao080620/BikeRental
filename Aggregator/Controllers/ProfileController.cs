using System.Security.Claims;
using Aggregator.Dto;
using Grpc.Net.ClientFactory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aggregator.Controllers;

[ApiController]
[Authorize]
[Route("[controller]/[action]")]
public class ProfileController : ControllerBase
{
    private readonly AccountServiceGrpc.AccountServiceGrpcClient _accountClient;
    private readonly BikeBookingServiceGrpc.BikeBookingServiceGrpcClient _bookingClient;

    public ProfileController(GrpcClientFactory clientFactory)
    {
        _accountClient = clientFactory.CreateClient<AccountServiceGrpc.AccountServiceGrpcClient>("AccountService");
        _bookingClient = clientFactory.CreateClient<BikeBookingServiceGrpc.BikeBookingServiceGrpcClient>("BikeBookingService");
    }
    
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;

        var accountInfo = await _accountClient.GetAccountInfoAsync(new GetAccountInfoRequest
        {
            Email = email
        });

        var accountRentingInfo = await _bookingClient.GetRentingInfoAsync(new GetRentingInfoRequest
        {
            Email = email
        });

        return Ok(new AccountProfileDto
        {
            FirstName = accountInfo.FirstName,
            LastName = accountInfo.LastName,
            Email = accountInfo.Email,
            Id = accountInfo.Id,
            PhoneNumber = accountInfo.PhoneNumber,
            Point = accountInfo.Point,
            TotalDistance = accountRentingInfo.TotalDistance,
            TotalRenting = accountRentingInfo.TotalRenting
        });
    }
}
