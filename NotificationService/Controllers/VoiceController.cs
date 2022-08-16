﻿using System.Security.Claims;
using System.Web;
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
    public IActionResult ReceivePhoneCall(VoiceRequest voiceRequest)
    {
        var response = new VoiceResponse();
        var gather = new Gather(
                new List<Gather.InputEnum> {Twilio.TwiML.Voice.Gather.InputEnum.Dtmf},
                numDigits: 1,
                action: new Uri("voice/gather", UriKind.Relative),
                method: HttpMethods.Post)
            .Play(new Uri("https://bike-rental-fe.s3.amazonaws.com/redirect_call.mp3"));
        response.Append(gather);
        response.Redirect(new Uri("voice", UriKind.Relative), HttpMethod.Post);
        
        return Content(response.ToString(), "application/xml");
    }
    
    [HttpGet]
    public IActionResult ReceiveSms()
    {
        var response = new MessagingResponse();
        var phoneNumber = HttpUtility.ParseQueryString(HttpContext.Request.QueryString.Value!);
        foreach (var test in phoneNumber)
        {
            Console.WriteLine(test);
        }
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
            Action = new Uri("HandleCompletedIncomingCall?email=testmanager@gmail.com", UriKind.Relative),
            Method = HttpMethod.Get,
            Record = Dial.RecordEnum.RecordFromAnswerDual,
            RecordingStatusCallback = new Uri("HandleCompletedRecording", UriKind.Relative),
            RecordingStatusCallbackMethod = HttpMethod.Get,
            RecordingStatusCallbackEvent = new List<Dial.RecordingEventEnum>
            {
                Dial.RecordingEventEnum.Completed
            }
        };


        // if (!string.IsNullOrEmpty(voiceRequest.Digits))
        // {
        //     switch (voiceRequest.Digits)
        //     {
        //         case "1":
        //             response.Say("You need support. We will help!");
        //             break;
        //         case "2":
        //             // var emails = (await _client.GetDirectorsAsync(new Empty())).Emails.Take(4).ToList();
        //             // emails.ForEach(email =>
        //             // {
        //             //     dial.Append(new Client().Identity(email));
        //             // });
        //             dial.Append(new Client().Identity("testmanager@gmail.com"));
        //             break;
        //         default:
        //             response.Say("Sorry, I don't understand that choice.").Pause();
        //             response.Redirect(new Uri("voice", UriKind.Relative), HttpMethod.Post);
        //             break;
        //     }
        // }
        // else
        // {
        //     // If no input was sent, redirect to the /voice route
        //     response.Redirect(new Uri("voice", UriKind.Relative), HttpMethod.Post);
        // }
        
        dial.Append(new Client().Identity("testmanager@gmail.com"));
        response.Append(dial);
        return Content(response.ToString(), "application/xml");
    }
    
    [HttpGet]
    [Route("[action]")]
    public IActionResult ForwardPhone()
    {
        var client = HttpUtility.ParseQueryString(HttpContext.Request.QueryString.Value!)["from"];
        var phoneNumber = HttpUtility.ParseQueryString(HttpContext.Request.QueryString.Value!)["to"]?
            .Replace(",", "");
        var response = new VoiceResponse();
        var dial = new Dial(callerId: "+19379091267");
        if (string.IsNullOrEmpty(phoneNumber))
        {
            
        }
        else
        {
            
        }
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

    [HttpGet]
    [Route("[action]")]
    public async Task<IActionResult> HandleCompletedIncomingCall([FromQuery] string email)
    {
        var queryString = HttpUtility.ParseQueryString(HttpContext.Request.QueryString.Value!);
        await _notificationRepository.AddCall(new Call
        {
            CallSid = queryString.Get("CallSid")!,
            CalledOn = DateTime.UtcNow,
            Duration = Convert.ToDouble(queryString["DialCallDuration"]),
            From = queryString.Get("From")!,
            To = queryString.Get("To")!,
            AnsweredBy = "Test@gmail.com",
            Status = queryString.Get("DialCallStatus")!,
            Direction = queryString.Get("Direction")!,
            RecordingUrl = queryString.Get("RecordingUrl"),
            CallerCountry = queryString.Get("FromCountry")!,
            CalledCountry =queryString.Get("ToCountry")!,
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
                await _notificationHub.PushUserAnswerPhoneCall(client.Replace("client:", string.Empty));
                break;
            default:
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
