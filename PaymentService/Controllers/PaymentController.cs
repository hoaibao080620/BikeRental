using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.DTO;
using PaymentService.Services;

namespace PaymentService.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class PaymentController : ControllerBase
{
    private readonly IStripeService _stripeService;

    public PaymentController(IStripeService stripeService)
    {
        _stripeService = stripeService;
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
}
