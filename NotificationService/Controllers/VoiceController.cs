using System.Security.Claims;
using System.Web;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.ClientFactory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.DAL;
using NotificationService.Hubs;
using NotificationService.Models;
using Twilio.AspNet.Common;
using Twilio.Jwt.AccessToken;
using Twilio.TwiML;
using Twilio.TwiML.Voice;
using HttpMethod = Twilio.Http.HttpMethod;

namespace NotificationService.Controllers;

[Route("[controller]")]
[ApiController]
[AllowAnonymous]
public class VoiceController : ControllerBase
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationHub _notificationHub;
    private readonly BikeServiceGrpc.BikeServiceGrpcClient _client;

    public VoiceController(INotificationRepository notificationRepository,
        GrpcClientFactory grpcClientFactory,
        INotificationHub notificationHub)
    {
        _notificationRepository = notificationRepository;
        _notificationHub = notificationHub;
        _client = grpcClientFactory.CreateClient<BikeServiceGrpc.BikeServiceGrpcClient>("BikeService");
    }
    
    [HttpPost]
    [Route("[action]")]
    [Consumes("application/x-www-form-urlencoded")]
    public IActionResult ReceivePhoneCall([FromForm] VoiceRequest voiceRequest)
    {
        var response = new VoiceResponse();

        var gather = new Gather(
                new List<Gather.InputEnum> {Twilio.TwiML.Voice.Gather.InputEnum.Dtmf},
                numDigits: 1,
                action: new Uri("gather", UriKind.Relative),
                method: HttpMethods.Post)
            .Play(new Uri("https://bike-rental-fe.s3.amazonaws.com/redirect_call.mp3"));
        response.Append(gather);
        response.Redirect(new Uri("receivePhoneCall", UriKind.Relative), HttpMethod.Post);
        return Content(response.ToString(), "application/xml");
    }

    [HttpPost]
    [Route("[action]")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> Gather([FromForm] VoiceRequest voiceRequest)
    {
        var response = new VoiceResponse();
        var dial = new Dial
        {
            Method = HttpMethod.Post,
            Record = Dial.RecordEnum.RecordFromAnswerDual,
            RecordingStatusCallback = new Uri("HandleCompletedRecording", UriKind.Relative),
            RecordingStatusCallbackMethod = HttpMethod.Get,
            RecordingStatusCallbackEvent = new List<Dial.RecordingEventEnum>
            {
                Dial.RecordingEventEnum.Completed
            }
        };

        var email = string.Empty;
        if (!string.IsNullOrEmpty(voiceRequest.Digits))
        {
            switch (voiceRequest.Digits)
            {
                case "1":
                    email = await GetManagerEmailToCall(voiceRequest.From);
                    email = "token@gmail.com";
                    break;
                case "2":
                    email = await GetDirectorEmailToCall();
                    email = "testmanager@gmail.com";
                    break;
                default:
                    response.Say("Sorry, I don't understand that choice.").Pause();
                    response.Redirect(new Uri("ReceivePhoneCall", UriKind.Relative), HttpMethod.Post);
                    break;
            }
        }
        else
        {
            // If no input was sent, redirect to the /voice route
            response.Redirect(new Uri("ReceivePhoneCall", UriKind.Relative), HttpMethod.Post);
        }
        
        dial.Append(new Client().Identity(email));
        dial.Action = new Uri($"HandleCompletedIncomingCall?email={email}", UriKind.Relative);
        response.Append(dial);
        return Content(response.ToString(), "application/xml");
    }

    private async Task<string> GetManagerEmailToCall(string from)
    {
        var manager = await _client.GetManagerEmailsAsync(new GetManagersByAccountEmailRequest
        {
            AccountPhone = from
        });

        var clientEmail = await GetClientEmail(manager);
        return clientEmail;
    }
    
    private async Task<string> GetDirectorEmailToCall()
    {
        var directors = await _client.GetDirectorsAsync(new Empty());
        var clientEmail = await GetClientEmail(directors);
        return clientEmail;
    }

    private async Task<string> GetClientEmail(GetManagersByAccountEmailResponse response)
    {
        var directorEmails = response.Managers.Select(x => x.Email);
        var callCountInDay = (await _notificationRepository
                .GetCalls(x => x.CalledOn >= DateTime.Now.Date && x.ManagerReceiver != null
                                                               && directorEmails.Contains(x.ManagerReceiver))).GroupBy(x => x.ManagerReceiver)
            .ToDictionary(x => x.Key!, x=> x.Count());
            
        string? clientEmail;
        if (callCountInDay.Any())
        {
            clientEmail = response.Managers
                .Select(x => new
                {
                    Receiver = x.Email,
                    Count = callCountInDay.GetValueOrDefault(x.Email),
                    ManagerCreatedOn = x.CreatedOn
                }).OrderBy(x => x.Count).ThenBy(x => x.ManagerCreatedOn).First().Receiver;
        }
        else
        {
            clientEmail = response.Managers.OrderBy(x => x.CreatedOn).Select(x => x.Email).First();
        }

        return clientEmail;
    } 


    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> ForwardPhone()
    {
        var client = HttpUtility.ParseQueryString(HttpContext.Request.QueryString.Value!)["from"];
        var phoneNumber = HttpUtility.ParseQueryString(HttpContext.Request.QueryString.Value!)["to"]?
            .Replace(",", "");
        var response = new VoiceResponse();
        var dial = new Dial(callerId: "+19133983864");
        dial.Number(phoneNumber,
            statusCallback: new Uri($"HandleCompletedOutgoingCall?client={client}", UriKind.Relative),
            statusCallbackMethod: HttpMethod.Post,
            statusCallbackEvent: new List<Number.EventEnum>
            {
                Number.EventEnum.Answered, 
                Number.EventEnum.Completed,
            });

        dial.Record = Dial.RecordEnum.RecordFromAnswerDual;
        dial.RecordingStatusCallback =
            new Uri("HandleCompletedRecording", UriKind.Relative);
        dial.RecordingStatusCallbackMethod = HttpMethod.Get;
        response.Append(dial);
        
        return Content(response.ToString(), "application/xml");
    }

    [HttpPost]
    [Route("[action]")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> HandleCompletedIncomingCall([FromForm] VoiceRequest voiceRequest, [FromQuery] string email)
    {
        await _notificationRepository.AddCall(new Call
        {
            CallSid = voiceRequest.CallSid,
            CalledOn = DateTime.UtcNow,
            Duration = Convert.ToDouble(voiceRequest.DialCallDuration),
            From = voiceRequest.From,
            To = voiceRequest.To,
            Status = voiceRequest.CallStatus,
            Direction = voiceRequest.Direction,
            RecordingUrl = voiceRequest.RecordingUrl,
            CallerCountry = voiceRequest.FromCountry,
            CalledCountry = voiceRequest.ToCountry,
            ManagerReceiver = email
        });
        
        var response = new VoiceResponse();
        response.Hangup();
        
        return Content(response.ToString(), "application/xml");
    }
    
    [HttpPost]
    [Route("[action]")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<IActionResult> HandleCompletedOutgoingCall([FromForm] VoiceRequest voiceRequest, [FromQuery] string client)
    {
        Console.WriteLine(voiceRequest.CallStatus);
        var response = new VoiceResponse();
        switch (voiceRequest.CallStatus)
        {
            case "in-progress":
                Console.WriteLine(client.Replace("client:", string.Empty));
                await _notificationHub.PushUserAnswerPhoneCall(client.Replace("client:", string.Empty));
                break;
            default:
                Console.WriteLine("Delete");
                await _notificationRepository.AddCall(new Call
                {
                    CallSid = voiceRequest.CallSid,
                    CalledOn = DateTime.UtcNow,
                    Duration = Convert.ToDouble(voiceRequest.DialCallDuration),
                    From = voiceRequest.From,
                    To = voiceRequest.To,
                    Status = voiceRequest.CallStatus,
                    Direction = "outbound",
                    RecordingUrl = voiceRequest.RecordingUrl,
                    CallerCountry = voiceRequest.FromCountry,
                    CalledCountry = voiceRequest.ToCountry,
                    ManagerCaller = client
                });
                response.Hangup();
                break;
        }
        
        return Content(response.ToString(), "application/xml");
    }
    
    [HttpGet]
    [Route("[action]")]
    public IActionResult HandleCompletedRecording()
    {
        return Ok();
    }
    
    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> GetCalls()
    {
        var calls = await _notificationRepository.GetCalls(_ => true);
        return Ok(calls);
    }
    
    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> GetManagerCalls()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        var calls = await _notificationRepository.GetCalls(c => c.ManagerCaller == email || c.ManagerReceiver == email);
        return Ok(calls);
    }

    [HttpGet]
    [Route("[action]")]
    [Authorize]
    public IActionResult GetToken()
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        var twilioAccountSid = Environment.GetEnvironmentVariable("Account_Sid");
        var twilioApiKey = Environment.GetEnvironmentVariable("Twilio_Api_Key");
        var twilioApiSecret = Environment.GetEnvironmentVariable("Twilio_Api_Secret");

        if (string.IsNullOrEmpty(twilioAccountSid))
        {
            throw new Exception();
        }

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

        return Ok(
            token.ToJwt()
        );
    }
}
