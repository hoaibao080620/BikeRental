using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace NotificationService.Services;

public class MessageService : IMessageService
{
    public async Task SendMessage(string to, string body)
    {
        var accountSid = Environment.GetEnvironmentVariable("Account_Sid");
        var authToken = Environment.GetEnvironmentVariable("Twilio_Account_Auth_Token");
        var messageServiceId = Environment.GetEnvironmentVariable("Twilio_Message_Id");
        TwilioClient.Init(accountSid, authToken);

        var messageOptions = new CreateMessageOptions( 
            new PhoneNumber($"+{to}"))
        {
            MessagingServiceSid = messageServiceId,
            Body = body
        };

        await MessageResource.CreateAsync(messageOptions); 
    }
}
