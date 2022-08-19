using System.Globalization;
using System.Text.Json;
using BikeRental.MessageQueue.Commands;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.Handlers;
using NotificationService.Consts;
using NotificationService.DAL;
using NotificationService.Hubs;
using NotificationService.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace NotificationService.MessageQueue.MessageQueueHandlers;

public class BikeCheckoutCommandHandler : IMessageQueueHandler
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationHub _notificationHub;

    public BikeCheckoutCommandHandler(INotificationRepository notificationRepository, INotificationHub notificationHub)
    {
        _notificationRepository = notificationRepository;
        _notificationHub = notificationHub;
    }
    
    public async Task Handle(string message)
    {
        var notificationCommand = JsonSerializer.Deserialize<BikeCheckedOut>(message);
        if(notificationCommand is null) return;

        var tasks = new List<Task>();
        foreach (var email in notificationCommand.ManagerEmails)
        {
            var notification = new Notification
            {
                NotificationEmail = email,
                IsOpen = false,
                IsSeen = false,
                IsHidden = false,
                NotificationContent = string.Format(
                    NotificationMessage.CheckoutMessage,
                    notificationCommand.AccountEmail,
                    notificationCommand.LicensePlate,
                    notificationCommand.CheckoutOn),
                CreatedOn = DateTime.UtcNow,
                NotificationType = NotificationType.Checkout
            };

            tasks.Add(_notificationRepository.AddNotification(notification));
            tasks.Add(_notificationHub.PushNotification(email, notification));
        }

        await Task.WhenAll(tasks);
        
        // var accountSid = Environment.GetEnvironmentVariable("Account_Sid");
        // var authToken = Environment.GetEnvironmentVariable("Twilio_Account_Auth_Token");
        // TwilioClient.Init(accountSid, authToken);
        //
        // var phoneNumber = notificationCommand.AccountEmail.Split("@")[0];
        //
        // var asiaTimezone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok");
        // var localDateTimeAtVietnam = TimeZoneInfo.ConvertTimeFromUtc(
        //     notificationCommand.CheckoutOn, asiaTimezone);
        //
        // await MessageResource.CreateAsync(
        //     body: $"Bạn vừa trả xe có mã {notificationCommand.BikeCode} thành công tại trạm " +
        //           $"{notificationCommand.BikeStationName} vào lúc {localDateTimeAtVietnam.ToString(CultureInfo.InvariantCulture)}." +
        //           $"Tổng chi phí của chuyến đi là {notificationCommand.RentingPoint} điểm," +
        //           " xin cảm ơn vì đã sử dụng dịch vụ của chúng tôi!",
        //     from: new Twilio.Types.PhoneNumber("+19379091267"),
        //     to: new Twilio.Types.PhoneNumber($"+{phoneNumber}")
        // );
    }
}
