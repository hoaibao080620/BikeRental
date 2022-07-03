using PaymentService.DTO;

namespace PaymentService.Services;

public interface IStripeService
{
    Task<PaymentCreatedDto> CreatePaymentIntent(PaymentDto paymentDto, string email);
    Task ProcessPaymentEvent(string eventJson, string signatureHeader);
}
