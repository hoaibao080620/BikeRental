using System.Net.Http.Headers;
using System.Security.Claims;
using BikeBookingService.BLL;
using BikeBookingService.Dtos.BikeOperation;
using BikeBookingService.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentry;

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
    
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> TestSentry()
    {
        SentrySdk.CaptureMessage("Bad request in TestSentry", SentryLevel.Fatal);
        return BadRequest();
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
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
        
        var isRenting = await _bikeTrackingValidation.IsAccountIsRentingBike(email);
        if (isRenting)
            return BadRequest("Bạn hiện tại đang thuê xe, xin vui lòng hoàn thành trả xe trước khi thuê xe mới!");
        
        // var isBikeCheckinWrongTime = await _bikeTrackingValidation.IsBikeCheckinOrCheckoutWrongTime(bikeCheckinDto.CheckinTime);
        // if (isBikeCheckinWrongTime) return BadRequest("Bạn không thể checkin trong khoảng thời gian sau 10h tối và trước 6h sáng!");

        // var isBikeHasEnoughPoint = await _bikeTrackingValidation.IsAccountHasEnoughPoint(
        //     email,
        //     Request.Headers[HeaderNames.Authorization]);
        //
        // if (!isBikeHasEnoughPoint) return BadRequest("Bạn phải có ít nhất 50 điểm trong tài khoản!");

        var isAccountHasBikeRentingPending = await _bikeTrackingValidation
            .IsAccountHasBikeRentingPending(email);
        
        if(isAccountHasBikeRentingPending) return BadRequest("Bạn hiện tại đang có 1 thanh toán đang trong quá trình xử lí" +
                                                             ", xin chờ trong giây lát và thử lại, xin cảm ơn!");
        
        var isAccountHasBikeRentingNotFullyPaid = await _bikeTrackingValidation
            .IsAccountHasBikeRentingNotFullyPaid(email);
        
        if(isAccountHasBikeRentingNotFullyPaid) return BadRequest("Bạn hiện đang có 1 lần thuê xe chưa hoàn thành thanh toán, " +
                                                                  "xin vui lòng nạp điểm và thử lại!");
        
        await _bikeTrackingBusinessLogic.BikeChecking(bikeCheckinDto, email);
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> Checkout(BikeCheckoutDto bikeCheckoutDto)
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;

        var isRenting = await _bikeTrackingValidation.IsAccountIsRentingBike(email);

        if (!isRenting) return BadRequest("Tài khoản của bạn đang không thuê xe nên không thể trả xe!");
        
        await _bikeTrackingBusinessLogic.BikeCheckout(bikeCheckoutDto, email);
        
        // var isBikeCheckoutWrongTime = await _bikeTrackingValidation.IsBikeCheckinOrCheckoutWrongTime(bikeCheckoutDto.CheckoutOn);
        // if (!isBikeCheckoutWrongTime) return Ok();
        //
        // await LockAccount(email, Request.Headers[HeaderNames.Authorization]);
        // return BadRequest("Tài khoản của bạn đã bị khóa và không thể thuê xe ở lần tới vì đã trả xe muộn (sau 22h), " +
        //                   "liên hệ với chúng tôi qua hotline để mở tải khoản. Xin cảm ơn!");

        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetRentingStatus()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
        var rentingStatus = await _bikeTrackingBusinessLogic.GetBikeRentingStatus(email);
    
        return Ok(rentingStatus);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetBikeBookingHistories()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;
        var rentingStatus = await _bikeTrackingBusinessLogic.GetBikeBookingHistories(email);
    
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
