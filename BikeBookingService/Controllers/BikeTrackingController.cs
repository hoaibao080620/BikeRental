﻿using System.Net.Http.Headers;
using System.Security.Claims;
using BikeBookingService.BLL;
using BikeBookingService.Dtos.BikeOperation;
using BikeBookingService.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace BikeBookingService.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Authorize]
public class BikeTrackingController : ControllerBase
{
    private readonly IBikeTrackingBusinessLogic _bikeTrackingBusinessLogic;
    private readonly IBikeTrackingValidation _bikeTrackingValidation;

    public BikeTrackingController(IBikeTrackingBusinessLogic bikeTrackingBusinessLogic, IBikeTrackingValidation bikeTrackingValidation)
    {
        _bikeTrackingBusinessLogic = bikeTrackingBusinessLogic;
        _bikeTrackingValidation = bikeTrackingValidation;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetBikeRentingHistory()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;

        var histories = await _bikeTrackingBusinessLogic.GetBikeRentingHistories(email);
        return Ok(histories);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetBikesTracking()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;

        var histories = await _bikeTrackingBusinessLogic.GetBikesTracking(email);
        return Ok(histories);
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateBikeLocation(BikeLocationDto bikeLocationDto)
    {
        await _bikeTrackingBusinessLogic.UpdateBikeLocation(bikeLocationDto);
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> Checking(BikeCheckinDto bikeCheckinDto)
    {
        var isBikeCheckinWrongTime = await _bikeTrackingValidation.IsBikeCheckinOrCheckoutWrongTime(bikeCheckinDto.CheckinTime);
        if (isBikeCheckinWrongTime) return BadRequest("You cannot checkin from 10pm to 6am!");
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;

        var isBikeHasEnoughPoint = await _bikeTrackingValidation.IsAccountHasEnoughPoint(
            email,
            Request.Headers[HeaderNames.Authorization]);

        if (!isBikeHasEnoughPoint) return BadRequest("You have to have at least 50 point!");

        var isAccountHasBikeRentingPending = await _bikeTrackingValidation
            .IsAccountHasBikeRentingPending(email);
        
        if(isAccountHasBikeRentingPending) return BadRequest("You have a bike renting which still processing, please wait a minutes!");
        
        var isAccountHasBikeRentingNotFullyPaid = await _bikeTrackingValidation
            .IsAccountHasBikeRentingNotFullyPaid(email);
        
        if(isAccountHasBikeRentingNotFullyPaid) return BadRequest("You have a bike renting which not filly paid, please add points to continue checking!");
        
        await _bikeTrackingBusinessLogic.BikeChecking(bikeCheckinDto, email);
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> Checkout(BikeCheckoutDto bikeCheckoutDto)
    {
        if (bikeCheckoutDto.BikeStationId is null)
            return BadRequest("You have to scan QR code of station before scan bike QR code");
        
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
        
        await _bikeTrackingBusinessLogic.BikeCheckout(bikeCheckoutDto, email);
        
        var isBikeCheckoutWrongTime = await _bikeTrackingValidation.IsBikeCheckinOrCheckoutWrongTime(bikeCheckoutDto.CheckoutOn);
        if (!isBikeCheckoutWrongTime) return Ok();
        
        await LockAccount(email, Request.Headers[HeaderNames.Authorization]);
        return BadRequest("Your account has been locked and cannot rent bike in next time due to late checkout after 22pm, " +
                          "please contact admin to unlock account!");
    }
    
    [HttpGet]
    public async Task<IActionResult> GetRentingStatus()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
        var rentingStatus = await _bikeTrackingBusinessLogic.GetBikeRentingStatus(email);
    
        return Ok(rentingStatus);
    }
    
    private async Task LockAccount(string accountEmail, string token)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", 
            token.Replace("Bearer", string.Empty));
        await httpClient.PutAsync(
            "https://bike-rental-account-service.herokuapp.com/account/" +
            $"lockAccount?accountEmail={accountEmail}", null);
    }
}
