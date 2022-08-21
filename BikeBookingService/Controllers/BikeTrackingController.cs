using System.Net.Http.Headers;
using System.Security.Claims;
using BikeBookingService.BLL;
using BikeBookingService.DAL;
using BikeBookingService.Dtos.BikeOperation;
using BikeBookingService.Validations;
using Grpc.Net.ClientFactory;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly BikeServiceGrpc.BikeServiceGrpcClient _bikeServiceGrpc;

    public BikeTrackingController(
        IBikeTrackingBusinessLogic bikeTrackingBusinessLogic,
        IBikeTrackingValidation bikeTrackingValidation,
        IUnitOfWork unitOfWork,
        GrpcClientFactory grpcClientFactory
    )
    {
        _bikeServiceGrpc = grpcClientFactory.CreateClient<BikeServiceGrpc.BikeServiceGrpcClient>("BikeService");
        _bikeTrackingBusinessLogic = bikeTrackingBusinessLogic;
        _bikeTrackingValidation = bikeTrackingValidation;
        _unitOfWork = unitOfWork;
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
    public async Task<IActionResult> GetAllBikesTracking()
    {
        var histories = await _bikeTrackingBusinessLogic.GetAllBikeTracking();
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

        var isBikeRenting = await _bikeTrackingValidation.IsBikeAlreadyRent(bikeCheckinDto.BikeCode);
        
        if (isBikeRenting) return BadRequest("Xe này hiện tại đang được sử dụng, xin vui lòng chọn xe khác!");
        
        var isRenting = await _bikeTrackingValidation.IsAccountIsRentingBike(email);
        if (isRenting)
            return BadRequest("Bạn hiện tại đang thuê xe, xin vui lòng hoàn thành trả xe trước khi thuê xe mới!");
        
        // var isBikeCheckinWrongTime = await _bikeTrackingValidation.IsBikeCheckinOrCheckoutWrongTime(bikeCheckinDto.CheckinTime);
        // if (isBikeCheckinWrongTime) return BadRequest("Bạn không thể checkin trong khoảng thời gian sau 10h tối và trước 6h sáng!");

        var (isBikeHasEnoughPoint, rentingPoint) = await _bikeTrackingValidation.IsAccountHasEnoughPoint(email);
        
        if (!isBikeHasEnoughPoint) return BadRequest($"Bạn phải có ít nhất {rentingPoint} điểm trong tài khoản!");

        var isAccountHasBikeRentingPending = await _bikeTrackingValidation
            .IsAccountHasBikeRentingPending(email);
        
        if(isAccountHasBikeRentingPending) return BadRequest("Bạn hiện tại đang có 1 thanh toán đang trong quá trình xử lí" +
                                                             ", xin chờ trong giây lát và thử lại, xin cảm ơn!");
        
        var isAccountHasBikeRentingNotFullyPaid = await _bikeTrackingValidation
            .IsAccountHasBikeRentingNotFullyPaid(email);
        
        if(isAccountHasBikeRentingNotFullyPaid) return BadRequest("Bạn hiện đang có 1 lần thuê xe chưa hoàn thành thanh toán, " +
                                                                  "xin vui lòng nạp điểm và thử lại!");
        
        await _bikeTrackingBusinessLogic.BikeChecking(bikeCheckinDto, email);
        return Ok("Bạn đã thuê xe thành công, chúc bạn có 1 chuyển đi vui vẻ!");
    }
    
    [HttpPost]
    public async Task<IActionResult> Checkout(BikeCheckoutDto bikeCheckoutDto)
    {
        try
        {
            var email = HttpContext.User.Claims.FirstOrDefault(x =>
                x.Type == ClaimTypes.NameIdentifier)!.Value;

            var isRenting = await _bikeTrackingValidation.IsAccountIsRentingBike(email);

            if (!isRenting) return BadRequest("Tài khoản của bạn đang không thuê xe nên không thể trả xe!");

            await _bikeTrackingBusinessLogic.BikeCheckout(bikeCheckoutDto, email);

            var isBikeCheckoutWrongTime = await _bikeTrackingValidation.IsBikeCheckinOrCheckoutWrongTime(bikeCheckoutDto.CheckoutOn);
            if (!isBikeCheckoutWrongTime)
            {
                return Ok("Bạn đã trả xe thành công, hẹn gặp lại bạn vào lần tới!");
            }
            
            // await LockAccount(email, Request.Headers[HeaderNames.Authorization]);
            return BadRequest("Tài khoản của bạn đã bị khóa và không thể thuê xe ở lần tới vì đã trả xe muộn (sau 22h), " +
                              "liên hệ với chúng tôi qua hotline để được hỗ trợ. Xin cảm ơn!");
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
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
            "https://bike-rental-account-service-1.herokuapp.com/account/" +
            $"lockAccount?accountEmail={accountEmail}", null);
    }
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAccountRentingStatus([FromQuery] string email)
    {
        var bikeStatus = await _bikeTrackingBusinessLogic.GetBikeRentingStatus(email);
        return Ok(bikeStatus);
    }
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> SyncData()
    {
        var bikes = await _unitOfWork.BikeRepository.Find(x => x.BikeStationCode == null && x.BikeStationId.HasValue);
        foreach (var bike in bikes.ToList())
        {
            var bikeStation = await _bikeServiceGrpc.GetBikeStationByCodeOrIdAsync(new GetBikeStationByCodeOrIdRequest
            {
                Id = bike.BikeStationId!.Value
            });

            bike.BikeStationCode = bikeStation.Code;
        }

        await _unitOfWork.SaveChangesAsync();
        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> TestMobileNotification()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value;

        await _bikeTrackingBusinessLogic.TestNotification(email);
        return Ok();
    }
}
