using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Twilio.Jwt.AccessToken;

namespace NotificationService.Controllers;

[ApiController]
[Route("[controller]")]
public class TokenController : ControllerBase
{
    [HttpGet]
    [Route("[action]")]
    [Authorize]
    public IActionResult GetToken()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        var twilioAccountSid = Environment.GetEnvironmentVariable("Twilio_Api_Key");
        var twilioApiKey = Environment.GetEnvironmentVariable("Twilio_Api_Secret");
        var twilioApiSecret = Environment.GetEnvironmentVariable("Account_Sid");

        var grant = new VoiceGrant
        {
            IncomingAllow = true,
            OutgoingApplicationSid = Environment.GetEnvironmentVariable("Out_Application_Sid")
        };

        var grants = new HashSet<IGrant>
        {
            grant
        };

        // Create an Access Token generator
        var token = new Token(
            twilioAccountSid,
            twilioApiKey,
            twilioApiSecret,
            email,
            grants: grants);

        return Ok(token.ToJwt());
    }
}
