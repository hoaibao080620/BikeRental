using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using Newtonsoft.Json;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace NotificationService.MessageQueue.MessageQueueHandlers;

public class PaymentSucceededEventHandler : IMessageQueueHandler
{
    public async Task Handle(string message)
    {
        var payload = JsonConvert.DeserializeObject<PaymentSucceeded>(message);
        if (payload is null) return;
        
        var accountSid = Environment.GetEnvironmentVariable("Account_Sid");
        var authToken = Environment.GetEnvironmentVariable("Twilio_Account_Auth_Token");
        TwilioClient.Init(accountSid, authToken);

        var phoneNumber = payload.Email.Split("@")[0];
        
        await MessageResource.CreateAsync(
            body: $"Bạn đã nạp thành công {payload.Amount} VNĐ vào tài khoản đăng ký ở Bike-Rental," +
                  " xin cảm ơn vì đã sử dụng dịch vụ của chúng tôi!",
            from: new Twilio.Types.PhoneNumber("+19379091267"),
            to: new Twilio.Types.PhoneNumber($"+{phoneNumber}")
        );
    }
}
