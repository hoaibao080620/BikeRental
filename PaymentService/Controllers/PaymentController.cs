using System.Security.Claims;
using BikeRental.MessageQueue.Events;
using BikeRental.MessageQueue.MessageType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.DTO;
using PaymentService.MessageQueue;
using PaymentService.Services;

namespace PaymentService.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class PaymentController : ControllerBase
{
    private readonly IStripeService _stripeService;
    private readonly IMessageQueuePublisher _messageQueuePublisher;

    public PaymentController(IStripeService stripeService, IMessageQueuePublisher messageQueuePublisher)
    {
        _stripeService = stripeService;
        _messageQueuePublisher = messageQueuePublisher;
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentDto paymentDto)
    {
        var email = HttpContext.User.Claims.FirstOrDefault(x =>
            x.Type == ClaimTypes.NameIdentifier)!.Value;

        var paymentCreatedDto = await _stripeService.CreatePaymentIntent(paymentDto, email);
        return Ok(paymentCreatedDto);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> StripePaymentWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        await _stripeService.ProcessPaymentEvent(json, Request.Headers["Stripe-Signature"]);
        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> TestSendText([FromQuery] string email)
    {
        await _messageQueuePublisher.PublishPaymentSucceededEvent(new PaymentSucceeded
        {
            Amount = 50000,
            Email = email,
            PaymentOn = DateTime.UtcNow,
            MessageType = MessageType.PaymentSucceeded
        });

        return Ok();
    }
}
