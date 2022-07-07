using Microsoft.AspNetCore.Mvc;
using Twilio;
using Twilio.Rest.Verify.V2.Service;
using UserService.Dtos;

namespace UserService.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ValidationController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SendVerificationSms([FromBody] PhoneVerificationDto phoneVerificationDto)
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
