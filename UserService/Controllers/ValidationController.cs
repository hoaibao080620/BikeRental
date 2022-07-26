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

    public ValidationController(IMongoService mongoService)
    {
        _mongoService = mongoService;
    }
    
    [HttpPost]
    public async Task<IActionResult> SendVerificationSms([FromBody] PhoneVerificationDto phoneVerificationDto)
    {
        var isPhoneNumberAlreadyExist = (await _mongoService
            .FindUser(x => x.PhoneNumber == phoneVerificationDto.PhoneNumber)).Any();

        if (isPhoneNumberAlreadyExist) return BadRequest("Số điện thoại này đã được sử dụng!");
        
        var accountSid = Environment.GetEnvironmentVariable("Twilio_Account_Sid");
        var authToken = Environment.GetEnvironmentVariable("Twilio_Account_Auth_Token");

        TwilioClient.Init(accountSid, authToken);

        await VerificationResource.CreateAsync(
            to: phoneVerificationDto.PhoneNumber,
            channel: "sms",
            pathServiceSid: Environment.GetEnvironmentVariable("Twilio_Verification_Service_Sid")
        );
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> ForgotPasswordVerification([FromBody] PhoneVerificationDto phoneVerificationDto)
    {
        var accountSid = Environment.GetEnvironmentVariable("Twilio_Account_Sid");
        var authToken = Environment.GetEnvironmentVariable("Twilio_Account_Auth_Token");

        TwilioClient.Init(accountSid, authToken);

        await VerificationResource.CreateAsync(
            to: phoneVerificationDto.PhoneNumber,
            channel: "sms",
            pathServiceSid: Environment.GetEnvironmentVariable("Twilio_Verification_Service_Sid")
        );
        
        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> CheckVerificationStatus([FromQuery] string verificationCode, string phoneNumber)
    {
        var accountSid = Environment.GetEnvironmentVariable("Twilio_Account_Sid");
        var authToken = Environment.GetEnvironmentVariable("Twilio_Account_Auth_Token");

        TwilioClient.Init(accountSid, authToken);
        
        var verificationCheck = await VerificationCheckResource.CreateAsync(
            to: $"+{phoneNumber}",
            code: verificationCode,
            pathServiceSid: Environment.GetEnvironmentVariable("Twilio_Verification_Service_Sid")
        );
        
        return Ok(new
        {
            verificationCheck.Status
        });
    }
}
