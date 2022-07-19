using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.MessageType;
using PaymentService.DTO;
using PaymentService.MessageQueue;
using Stripe;

namespace PaymentService.Services;

public class StripeService : IStripeService
{
    private readonly IConfiguration _configuration;
    private readonly IMessageQueuePublisher _messageQueuePublisher;

    public StripeService(IConfiguration configuration, IMessageQueuePublisher messageQueuePublisher)
    {
        _configuration = configuration;
        _messageQueuePublisher = messageQueuePublisher;
    }
    
    public async Task<PaymentCreatedDto> CreatePaymentIntent(PaymentDto paymentDto, string email)
    {
        var service = new CustomerService();
        var customer = await service.CreateAsync(new CustomerCreateOptions
        {
            Email = email
        });
        
        var options = new PaymentIntentCreateOptions
        {
            Amount = paymentDto.Point,
            Currency = "vnd",
            PaymentMethodTypes = new List<string>
            {
                "card"
            },
            ReceiptEmail = email
        };

        var ephemeralService = new EphemeralKeyService();
        var ephemeralKey = await ephemeralService.CreateAsync(new EphemeralKeyCreateOptions
        {
            Customer = customer.Id
        });
        
        var paymentIntentService = new PaymentIntentService();
        var paymentIntent = await paymentIntentService.CreateAsync(options);
        
        return new PaymentCreatedDto
        {
            PaymentIntent = paymentIntent.ClientSecret,
            EphemeralKey = ephemeralKey.Secret,
            Customer = customer.Id,
            PublishableKey = _configuration["Stripe:PublishableKey"]
        };
    }

    public async Task ProcessPaymentEvent(string eventJson, string signatureHeader)
    {
        var endpointSecret = _configuration["Stripe:WebhookSecret"];
        var stripeEvent = EventUtility.ConstructEvent(eventJson, signatureHeader, endpointSecret);
        switch (stripeEvent.Type)
        {
            case Events.PaymentIntentSucceeded:
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                var paymentCharge = paymentIntent!.Charges.Data.FirstOrDefault();
                await _messageQueuePublisher.PublishPaymentSucceededEvent(new PaymentSucceeded
                {
                    Amount = paymentIntent.Amount,
                    Email = paymentIntent.Customer?.Email ?? paymentCharge!.ReceiptEmail,
                    PaymentOn = DateTime.Now,
                    MessageType = MessageType.PaymentSucceeded
                });
                break;
            }
            // case Events.PaymentIntentPaymentFailed:
            // {
            //     var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            //     var paymentCharge = paymentIntent!.Charges.Data.FirstOrDefault()!;
            //     await _messageQueuePublisher.PublishPaymentFailedEvent(new PaymentFailed
            //     {
            //         Amount = paymentIntent.Amount,
            //         Email = paymentIntent.Customer?.Email ?? paymentCharge.ReceiptEmail,
            //         FailedOn = DateTime.Now,
            //         FailureMessage = paymentCharge.FailureMessage,
            //         MessageType = MessageType.PaymentFailed
            //     });
            //     break;
            // }
            // case Events.PaymentIntentRequiresAction:
            // {
            //     var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            //     var paymentCharge = paymentIntent!.Charges.Data.FirstOrDefault()!;
            //     await _messageQueuePublisher.PublishPaymentFailedEvent(new PaymentFailed
            //     {
            //         Amount = paymentIntent.Amount,
            //         Email = paymentIntent.Customer?.Email ?? paymentCharge.ReceiptEmail,
            //         FailedOn = DateTime.Now,
            //         FailureMessage = paymentCharge.FailureMessage,
            //         MessageType = MessageType.PaymentFailed
            //     });
            //     break;
            // }
            default:
                Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                break;
        }
    }
}
