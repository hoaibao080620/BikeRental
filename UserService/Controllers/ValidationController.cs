using Microsoft.AspNetCore.Mvc;
using Twilio;
using Twilio.Rest.Verify.V2.Service;
using UserService.DataAccess;
using UserService.Dtos;

namespace UserService.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ValidationController : ControllerBase
{
    private readonly IMongoService _mongoService;
    private readonly string? _twilioVerificationService;

    public ValidationController(IMongoService mongoService)
    {
        _mongoService = mongoService;
        var accountSid = Environment.GetEnvironmentVariable("Twilio_Account_Sid");
        var authToken = Environment.GetEnvironmentVariable("Twilio_Account_Auth_Token");
        _twilioVerificationService = Environment.GetEnvironmentVariable("Twilio_Verification_Service_Sid");
        TwilioClient.Init(accountSid, authToken);
    }
    
    [HttpPost]
    public async Task<IActionResult> SendVerificationSms([FromBody] PhoneVerificationDto phoneVerificationDto)
    {
        var isPhoneNumberAlreadyExist = (await _mongoService
            .FindUser(x => x.PhoneNumber == phoneVerificationDto.PhoneNumber)).Any();

        if (isPhoneNumberAlreadyExist) return BadRequest("Số điện thoại này đã được sử dụng!");

        await VerificationResource.CreateAsync(
            to: phoneVerificationDto.PhoneNumber,
            channel: "sms",
            pathServiceSid: _twilioVerificationService
        );
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> ForgotPasswordVerification([FromBody] PhoneVerificationDto phoneVerificationDto)
    {
        await VerificationResource.CreateAsync(
            to: phoneVerificationDto.PhoneNumber,
            channel: "sms",
            pathServiceSid: _twilioVerificationService
        );
        
        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> CheckVerificationStatus([FromQuery] string verificationCode, string phoneNumber)
    {
        var verificationCheck = await VerificationCheckResource.CreateAsync(
            to: $"+{phoneNumber}",
            code: verificationCode,
            pathServiceSid: _twilioVerificationService
        );
        
        return Ok(new
        {
            verificationCheck.Status
        });
    }
}
