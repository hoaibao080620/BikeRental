using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.DTO;
using Stripe;

namespace PaymentService.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Authorize]
public class PaymentController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentDto paymentDto)
    {
        var service = new CustomerService();
        var customer = await service.CreateAsync(new CustomerCreateOptions
        {
            Email = HttpContext.User.Claims.FirstOrDefault(x => 
            x.Type == ClaimTypes.NameIdentifier)!.Value
        });
        
        var options = new PaymentIntentCreateOptions
        {
            Amount = paymentDto.Point * 1000,
            Currency = "usd",
            PaymentMethodTypes = new List<string>
            {
                "card"
            }
        };

        var ephemeralService = new EphemeralKeyService();
        var ephemeralKey = await ephemeralService.CreateAsync(new EphemeralKeyCreateOptions
        {
            Customer = customer.Id
        });
        
        var paymentIntentService = new PaymentIntentService();
        var paymentIntent = await paymentIntentService.CreateAsync(options);
        
        return Ok(new PaymentCreatedDto
        {
            PaymentIntent = paymentIntent.ClientSecret,
            EphemeralKey = ephemeralKey.Secret,
            Customer = customer.Id,
            PublishableKey = "pk_test_51I9C85F9AODfnpB1SpivIpPXMT8G7QtPUEWx92hFJThm6CqHBi4IzEadWnuRhBr8IQR2mErgHUbybSY8LFFYrGyC00VK4f0VpC"
        });
    }

    [HttpPost]
    public async Task<IActionResult> PaymentWebhook()
    {
        const string endpointSecret = "whsec_wvebql7TdSXY9NieEh32n70gGcJSjncX";
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(json,
                Request.Headers["Stripe-Signature"], endpointSecret);

            switch (stripeEvent.Type)
            {
                case Events.PaymentIntentSucceeded:
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    Console.WriteLine("PaymentIntent was successful!");
                    break;
                }
                case Events.PaymentIntentPaymentFailed:
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    Console.WriteLine("paymentIntent was attached to a Customer!");
                    break;
                }
                // ... handle other event types
                default:
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                    break;
            }

            return Ok();
        }
        catch (StripeException)
        {
            return BadRequest();
        }
    }
}
